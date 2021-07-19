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

namespace net.atos.daf.ct2.rfmsdataservice.Controllers
{

    [ApiController]
    [Route("rfms")]
    public class RfmsDataServiceController : ControllerBase
    {
        private readonly ILogger<RfmsDataServiceController> _logger;
        private readonly IRfmsManager _rfmsManager;
        private readonly IAuditTraillib _auditTrail;
        private readonly IAccountManager _accountManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IConfiguration _configuration;


        public RfmsDataServiceController(IAccountManager accountManager,
                                         ILogger<RfmsDataServiceController> logger,
                                         IAuditTraillib auditTrail,
                                         IRfmsManager rfmsManager,
                                         IVehicleManager vehicleManager,
                                         IConfiguration configuration)
        {
            _logger = logger;
            _auditTrail = auditTrail;
            this._rfmsManager = rfmsManager;
            this._accountManager = accountManager;
            this._vehicleManager = vehicleManager;
            this._configuration = configuration;
        }


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

                //Validation with respect to xCorrelationId needs to be considered in later stage
                this.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues xCorrelationId);

                if (!this.Request.Headers.ContainsKey("Accept") ||
                    (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept");

                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "rFMS Vehicle Data Service", "rFMS Vehicle Data Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get Vehicles method rFMS vehicle data service", 1, 2, lastVin, 0, 0);

                if (acceptHeader.Any(x => x.Trim().Equals(RFMSResponseTypeConstants.JSON, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = RFMSResponseTypeConstants.JSON;

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

                    ResponseObject responseObject = new ResponseObject();
                    VehicleResponse vehicleResponseObject = new VehicleResponse();
                    vehicleResponseObject.Vehicles = new List<Entity.Vehicle>();
                    int vehicleCnt = 0;

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

                        vehicleObj.Type = item.Type;
                        vehicleObj.Model = item.Model;
                        vehicleObj.PossibleFuelType = item.PossibleFuelType;
                        vehicleObj.EmissionLevel = item.EmissionLevel;
                        vehicleObj.TellTaleCode = item.TellTaleCode;
                        vehicleObj.ChassisType = item.ChassisType;
                        vehicleObj.NoOfAxles = item.NoOfAxles;
                        vehicleObj.TotalFuelTankVolume = item.TotalFuelTankVolume;
                        vehicleObj.TachographType = item.TachographType;
                        vehicleObj.GearboxType = item.GearboxType;
                        vehicleObj.BodyType = item.BodyType;
                        vehicleObj.DoorConfiguration = item.DoorConfiguration;
                        vehicleObj.HasRampOrLift = item.HasRampOrLift;
                        vehicleObj.AuthorizedPaths = item.AuthorizedPaths;
                        vehicleResponseObject.Vehicles.Add(vehicleObj);
                        vehicleCnt++;
                    }
                    if (rfmsVehicles.MoreDataAvailable)
                    {
                        responseObject.MoreDataAvailable = true;
                        responseObject.MoreDataAvailableLink = "/rfms/vehicles?lastVin='" + rfmsVehicles.Vehicles.Last().Vin + "'";
                    }
                    else
                        responseObject.MoreDataAvailable = false;
                    responseObject.VehicleResponse = vehicleResponseObject;
                    return Ok(responseObject);
                }
                return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(lastVin));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing rFMS Vehicle data.");
                return StatusCode(500, string.Empty);
            }
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

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = "INVALID_PARAMETER",
                Value = value
            });
        }
        [HttpGet]
        [Route("vehicleposition")]
        [Authorize(Policy = AccessPolicies.RFMS_VEHICLE_POSITION_ACCESS_POLICY)]
        public async Task<IActionResult> GetVehiclePosition()
        {
            try
            {
                //long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                //await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Rfms Vehicle Position Service", "Rfms Vehicle Position Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get vehicle position method vehicle position service", 1, 2, JsonConvert.SerializeObject(rfmsVehiclePositionRequest), 0, 0);
                //var responce = new RfmsVehiclePosition();
                //responce = await _rfmsManager.GetVehiclePosition(rfmsVehiclePositionRequest);
                return Ok("Test");

            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Rfms Vehicle data.");
                return StatusCode(500, string.Empty);
            }
        }


    }
}
