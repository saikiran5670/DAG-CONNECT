using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.rfms;
using net.atos.daf.ct2.rfms.response;
using net.atos.daf.ct2.rfmsdataservice.CustomAttributes;
using net.atos.daf.ct2.rfmsdataservice.Entity;
using net.atos.daf.ct2.utilities;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using net.atos.daf.ct2.vehicle;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.rfms.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.rfmsdataservice.Controllers
{

    [ApiController]
    [Route("rfms")]
    public class RfmsDataServiceController : ControllerBase
    {
        #region Constructor & Initialization Variables
        private readonly ILogger<RfmsDataServiceController> _logger;
        private readonly IRfmsManager _rfmsManager;
        private readonly IAuditTraillib _auditTrail;
        private readonly IAccountManager _accountManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RfmsDataServiceController(IAccountManager accountManager,
                                         ILogger<RfmsDataServiceController> logger,
                                         IAuditTraillib auditTrail,
                                         IRfmsManager rfmsManager,
                                         IVehicleManager vehicleManager,
                                         IConfiguration configuration,
                                         IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _auditTrail = auditTrail;
            this._rfmsManager = rfmsManager;
            this._accountManager = accountManager;
            this._vehicleManager = vehicleManager;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region rFMS 3.0 Vehicles API Endpoint
        [HttpGet]
        [Route("vehicles")]
        [Authorize(Policy = AccessPolicies.RFMS_VEHICLE_DATA_ACCESS_POLICY)]
        public async Task<IActionResult> GetVehicles([FromQuery] string lastVin)
        {
            try
            {
                var selectedType = string.Empty;
                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

                this.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues xCorrelationId);

                if (!this.Request.Headers.ContainsKey("X-Correlation-ID") || (this.Request.Headers.ContainsKey("X-Correlation-ID") && acceptHeader.Count() == 0))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "X-Correlation-ID", "INVALID_PARAMETER");

                if (!this.Request.Headers.ContainsKey("Accept") || (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept", "INVALID_PARAMETER");

                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "rFMS Vehicle Data Service", "rFMS Vehicle Data Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get Vehicles method rFMS vehicle data service xCorrelationId:" + xCorrelationId, 0, 0, lastVin, 0, 0);

                if (acceptHeader.Any(x => x.Trim().Equals(RFMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = RFMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON;
                else
                    return GenerateErrorResponse(HttpStatusCode.NotAcceptable, "Accept", "NOT_ACCEPTABLE value in accept - " + acceptHeader);

                var isValid = ValidateParameter(ref lastVin, out bool moreData);

                if (isValid)
                {
                    var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                    var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());
                    var orgs = await _accountManager.GetAccountOrg(account.Id);

                    //Get Threshold Value from Congifurations
                    var thresholdRate = _configuration.GetSection("rfms3.vehicles").GetSection("DataThresholdValue").Value;

                    int thresholdValue = Convert.ToInt32(thresholdRate);

                    RfmsVehicles rfmsVehicles = new RfmsVehicles();

                    rfmsVehicles = await _rfmsManager.GetVehicles(lastVin, thresholdValue, account.Id, orgs.First().Id);

                    VehicleResponseObject responseObject = MapVehiclesRecord(rfmsVehicles);


                    return Ok(responseObject);
                }
                return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(lastVin), "INVALID_PARAMETER");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing rFMS Vehicle data.");
                return StatusCode(500, string.Empty);
            }
        }
        #endregion

        #region rFMS 3.0 Vehicles Positions API Endpoint
        [HttpGet]
        [Route("vehiclepositions")]
        [Authorize(Policy = AccessPolicies.RFMS_VEHICLE_POSITION_ACCESS_POLICY)]
        public async Task<IActionResult> GetVehiclePositions([FromQuery] string datetype,
                                                             [FromQuery] string starttime,
                                                             [FromQuery] string stoptime,
                                                             [FromQuery] string vin,
                                                             [FromQuery] bool latestOnly,
                                                             [FromQuery] string triggerFilter,
                                                             [FromQuery] string lastVin)
        {
            try
            {
                var selectedType = string.Empty;

                #region Request Header & Querystring Parameters checks
                this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

                if (!this.Request.Headers.ContainsKey("Accept") || (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept", "INVALID_PARAMETER");

                //Check if no mandatory parameters are supplied at all & raise error
                if (!latestOnly && string.IsNullOrEmpty(starttime))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "latestOnly or StartTime", "INVALID_SUPPLIED_PARAMETERS, LatestOnly or StartTime parameters are mandatory to get result");

                //If latestOnly and starttime and/or stoptime are set, a HTTP 400 error will be returned indicating that the parameters supplied are invalid.
                if (latestOnly && (!string.IsNullOrEmpty(starttime) || !string.IsNullOrEmpty(stoptime)))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(latestOnly), "INVALID_SUPPLIED_PARAMETERS, LatestOnly cannot be combined with StartTime and/or StopTime");

                if (acceptHeader.Any(x => x.Trim().Equals(RFMSResponseTypeConstants.ACCEPT_TYPE_VEHICLE_POSITION_JSON, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = RFMSResponseTypeConstants.ACCEPT_TYPE_VEHICLE_POSITION_JSON;
                else
                    return GenerateErrorResponse(HttpStatusCode.NotAcceptable, "Accept", "NOT_ACCEPTABLE value in accept - " + acceptHeader);

                #endregion

                var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());
                var orgs = await _accountManager.GetAccountOrg(account.Id);

                //Get Threshold Value from Congifurations
                var thresholdRate = _configuration.GetSection("rfms3.vehiclepositions").GetSection("DataThresholdValue").Value;
                int thresholdValue = Convert.ToInt32(thresholdRate);

                RfmsVehiclePositionRequest vehiclePositionRequest = MapVehiclePositionRequest(datetype,
                                                                             starttime,
                                                                             stoptime,
                                                                             vin,
                                                                             latestOnly,
                                                                             triggerFilter,
                                                                             lastVin);

                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Rfms Vehicle Position Service", "Rfms Vehicle Position Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get vehicle position method vehicle position service", 0, 0, JsonConvert.SerializeObject(vehiclePositionRequest), 0, 0);

                vehiclePositionRequest.OrgId = orgs.First().Id;
                vehiclePositionRequest.AccountId = account.Id;
                vehiclePositionRequest.ThresholdValue = thresholdValue;

                var isValid = ValidateVehiclePositionParameters(ref vehiclePositionRequest, out string field);
                if (isValid)
                {
                    var response = new RfmsVehiclePosition();
                    response = await _rfmsManager.GetVehiclePosition(vehiclePositionRequest);
                    return Ok(MapVehiclePositionResponse(response));
                }
                return GenerateErrorResponse(HttpStatusCode.BadRequest, field, "INVALID_PARAMETER");
            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Rfms Vehicle data.");
                return StatusCode(500, string.Empty);
            }
        }
        #endregion

        #region Mappers
        private VehiclePositionResponseObject MapVehiclePositionResponse(RfmsVehiclePosition rfmsVehiclePosition)
        {
            DateTime currentdatetime = DateTime.Now;

            List<VehiclePositions> vehiclePositions = new List<VehiclePositions>();

            foreach (var vehicle in rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions)
            {
                Entity.TachoDriverIdentification tachoDriverIdentification = new Entity.TachoDriverIdentification()
                {
                    DriverIdentification = vehicle.TriggerType.DriverId.TachoDriverIdentification.DriverIdentification,
                    CardIssuingMemberState = vehicle.TriggerType.DriverId.TachoDriverIdentification.CardIssuingMemberState,
                    CardRenewalIndex = vehicle.TriggerType.DriverId.TachoDriverIdentification.CardRenewalIndex,
                    CardReplacementIndex = vehicle.TriggerType.DriverId.TachoDriverIdentification.CardReplacementIndex,
                    DriverAuthenticationEquipment = vehicle.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment
                };

                Entity.OemDriverIdentification oemDriverIdentification = new Entity.OemDriverIdentification()
                {
                    IdType = vehicle.TriggerType.DriverId.OemDriverIdentification.IdType,
                    OemDriverId = vehicle.TriggerType.DriverId.OemDriverIdentification.DriverIdentification
                };

                DriverIdObject driverIdObject = new DriverIdObject()
                {
                    TachoDriverIdentification = tachoDriverIdentification,
                    OemDriverIdentification = oemDriverIdentification
                };

                TellTaleObject tellTaleObject = new TellTaleObject()
                {
                    State = vehicle.TriggerType.TellTaleInfo.State,
                    TellTale = vehicle.TriggerType.TellTaleInfo.TellTale,
                    OemTellTale = vehicle.TriggerType.TellTaleInfo.OemTellTale
                };

                TriggerObject triggerObject = new TriggerObject()
                {
                    Context = vehicle.TriggerType.Context,
                    PtoId = vehicle.TriggerType.PtoId,
                    TriggerInfo = vehicle.TriggerType.TriggerInfo,
                    DriverId = driverIdObject,
                    TriggerType = vehicle.TriggerType.Type,
                    TellTaleInfo = tellTaleObject
                };

                Entity.GNSSPositionObject gNSSPositionObject = new GNSSPositionObject()
                {
                    Altitude = vehicle.GnssPosition.Altitude,
                    Heading = vehicle.GnssPosition.Heading,
                    Latitude = vehicle.GnssPosition.Latitude,
                    Longitude = vehicle.GnssPosition.Longitude,
                    PositionDateTime = vehicle.GnssPosition.PositionDateTime.ToString("yyyy-MM-ddThh:mm:ss.fffZ"),
                    Speed = vehicle.GnssPosition.Speed
                };

                VehiclePositions vehiclePosition = new VehiclePositions()
                {
                    CreatedDateTime = vehicle.CreatedDateTime.ToString("yyyy-MM-ddThh:mm:ss.fffZ"),
                    ReceivedDateTime = vehicle.ReceivedDateTime.ToString("yyyy-MM-ddThh:mm:ss.fffZ"),
                    TachographSpeed = vehicle.TachographSpeed,
                    WheelBasedSpeed = vehicle.WheelBasedSpeed,
                    TriggerType = triggerObject,
                    Vin = vehicle.Vin,
                    GnssPosition = gNSSPositionObject
                };

                vehiclePositions.Add(vehiclePosition);
            }

            Entity.VehiclePositionResponse vehiclePositionResponse = new Entity.VehiclePositionResponse()
            {
                VehiclePositions = vehiclePositions
            };

            return new VehiclePositionResponseObject()
            {
                RequestServerDateTime = currentdatetime.ToString("yyyy-MM-ddThh:mm:ss.fffZ"),
                MoreDataAvailable = rfmsVehiclePosition.MoreDataAvailable,
                MoreDataAvailableLink = rfmsVehiclePosition.MoreDataAvailableLink,
                VehiclePositionResponse = vehiclePositionResponse
            };
        }

        private RfmsVehiclePositionRequest MapVehiclePositionRequest(string datetype,
                                                         string starttime,
                                                         string stoptime,
                                                         string vin,
                                                         bool latestOnly,
                                                         string triggerFilter,
                                                         string lastVin)
        {
            RfmsVehiclePositionRequest vehiclePositionRequest = new RfmsVehiclePositionRequest();

            if (string.IsNullOrEmpty(datetype))
                vehiclePositionRequest.Type = DateType.Received;
            else
                vehiclePositionRequest.Type = DateType.Created;
            vehiclePositionRequest.LastVin = lastVin;
            vehiclePositionRequest.LatestOnly = latestOnly;
            vehiclePositionRequest.StartTime = starttime;
            vehiclePositionRequest.StopTime = stoptime;
            vehiclePositionRequest.TriggerFilter = triggerFilter;
            vehiclePositionRequest.Vin = vin;
            return vehiclePositionRequest;
        }

        private VehicleResponseObject MapVehiclesRecord(RfmsVehicles rfmsVehicles)
        {
            VehicleResponseObject responseObject = new VehicleResponseObject();
            VehicleResponse vehicleObject = new VehicleResponse();
            vehicleObject.Vehicles = new List<Entity.Vehicle>();
            int vehicleCnt = 0;

            //validate authorize paths
            List<string> lstAuthorizedPaths = new List<string>();
            if (_httpContextAccessor.HttpContext.Items["AuthorizedPaths"] != null)
            {
                lstAuthorizedPaths = (List<string>)_httpContextAccessor.HttpContext.Items["AuthorizedPaths"];
            }

            foreach (var item in rfmsVehicles.Vehicles)
            {
                Entity.Vehicle vehicleObj = new Entity.Vehicle();
                vehicleObj.Vin = item.Vin;
                vehicleObj.CustomerVehicleName = item.CustomerVehicleName;
                vehicleObj.Brand = item.Brand;

                if (item.ProductionDate != null)
                {
                    Entity.ProductionDate prdDate = new Entity.ProductionDate();
                    prdDate.Day = item.ProductionDate.Day;
                    prdDate.Month = item.ProductionDate.Month;
                    prdDate.Year = item.ProductionDate.Year;
                    vehicleObj.ProductionDate = prdDate;
                }
                //Below commented fields as due to no db mapping provided by database team and is currently seeks clarification from DAF
                vehicleObj.Type = item.Type;
                vehicleObj.Model = item.Model;
                vehicleObj.PossibleFuelType = item.PossibleFuelType;
                vehicleObj.EmissionLevel = item.EmissionLevel;
                //vehicleObj.TellTaleCode = item.TellTaleCode;
                vehicleObj.ChassisType = item.ChassisType;
                vehicleObj.NoOfAxles = item.NoOfAxles;
                vehicleObj.TotalFuelTankVolume = item.TotalFuelTankVolume;
                //vehicleObj.TachographType = item.TachographType;
                vehicleObj.GearboxType = item.GearboxType;
                //vehicleObj.BodyType = item.BodyType;
                //vehicleObj.DoorConfiguration = item.DoorConfiguration;
                //vehicleObj.HasRampOrLift = item.HasRampOrLift;
                vehicleObj.AuthorizedPaths = lstAuthorizedPaths;
                vehicleObject.Vehicles.Add(vehicleObj);
                vehicleCnt++;
            }
            if (rfmsVehicles.MoreDataAvailable)
            {
                responseObject.MoreDataAvailable = true;
                responseObject.MoreDataAvailableLink = "/rfms/vehicles?lastVin='" + rfmsVehicles.Vehicles.Last().Vin + "'";
            }
            else
                responseObject.MoreDataAvailable = false;
            responseObject.VehicleResponse = vehicleObject;

            return responseObject;
        }
        #endregion

        #region Validators
        private bool ValidateVehiclePositionParameters(ref RfmsVehiclePositionRequest vehiclePositionRequest, out string field)
        {
            field = string.Empty;

            //Validate StartTime
            if (!string.IsNullOrEmpty(vehiclePositionRequest.StartTime))
            {
                if (!DateTime.TryParse(vehiclePositionRequest.StartTime, out _))
                {
                    field = nameof(vehiclePositionRequest.StartTime);
                    return false;
                }
            }

            //Validate StopTime
            if (!string.IsNullOrEmpty(vehiclePositionRequest.StopTime))
            {
                if (!DateTime.TryParse(vehiclePositionRequest.StopTime, out _) || string.IsNullOrEmpty(vehiclePositionRequest.StartTime))
                {
                    field = nameof(vehiclePositionRequest.StopTime);
                    return false;
                }
            }

            //Validate VIN
            if (!string.IsNullOrEmpty(vehiclePositionRequest.Vin))
            {
                string vin = vehiclePositionRequest.Vin;
                Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(vin));
                if (vinNo.Result == 0)
                {
                    field = nameof(vehiclePositionRequest.Vin);
                    return false;
                }
            }

            //Validate Last VIN
            if (!string.IsNullOrEmpty(vehiclePositionRequest.LastVin))
            {
                string vin = vehiclePositionRequest.LastVin;
                Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(vin));
                if (vinNo.Result == 0)
                {
                    field = nameof(vehiclePositionRequest.LastVin);
                    return false;
                }
            }
            return true;
        }

        private bool ValidateParameter(ref string lastVin, out bool moreData)
        {
            moreData = true;
            if (string.IsNullOrEmpty(lastVin))
            {
                moreData = false;
            }
            else
            {
                //Validate Vin no from Db
                string lastVinNo = lastVin;
                Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(lastVinNo));
                if (vinNo.Result == 0)
                {
                    moreData = false;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Error Generator
        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value, string message)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = message,
                Value = value
            });
        }
        #endregion
    }
}
