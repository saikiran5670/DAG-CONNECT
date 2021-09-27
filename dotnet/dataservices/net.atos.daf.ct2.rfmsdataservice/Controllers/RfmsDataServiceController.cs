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
        private readonly Mapper _mapper;
        internal int AccountId { get; set; }
        internal int OrgId { get; set; }


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
            _mapper = new Mapper(this._httpContextAccessor, _vehicleManager);

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

                var isValid = _mapper.ValidateParameter(ref lastVin, out bool moreData);

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

                    VehicleResponseObject responseObject = _mapper.MapVehiclesRecord(rfmsVehicles);


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
                await GetUserDetails();
                var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(AccountId, OrgId);
                if (visibleVehicles.Count() == 0)
                {

                    var message = string.Format(RFMSResponseTypeConstants.GET_VIN_VISIBILITY_FAILURE_MSG, AccountId, OrgId);
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "VIN_VISIBILITY_FAILURE", message);

                }
                var requestFilter = new RfmsVehiclePositionStatusFilter() { Vin = vin, LastVin = lastVin, LatestOnly = latestOnly, StartTime = starttime, StopTime = stoptime, TriggerFilter = triggerFilter, Type = datetype };

                var selectedType = ValidateHeaderRequest(requestFilter, RFMSResponseTypeConstants.ACCEPT_TYPE_VEHICLE_POSITION_JSON, out bool isHeaderValid);
                if (!isHeaderValid)
                    return selectedType;


                //var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                //var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());
                //var orgs = await _accountManager.GetAccountOrg(account.Id);

                //Get Threshold Value from Congifurations
                var thresholdRate = _configuration.GetSection("rfms3.vehiclepositions").GetSection("DataThresholdValue").Value;
                int thresholdValue = Convert.ToInt32(thresholdRate);

                RfmsVehiclePositionRequest vehiclePositionRequest = _mapper.MapVehiclePositionRequest(requestFilter);

                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Rfms Vehicle Position Service", "Rfms Vehicle Position Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get vehicle position method vehicle position service", 0, 0, JsonConvert.SerializeObject(vehiclePositionRequest), 0, 0);

                vehiclePositionRequest.OrgId = OrgId;
                vehiclePositionRequest.AccountId = AccountId;
                vehiclePositionRequest.ThresholdValue = thresholdValue;

                var isValid = _mapper.ValidateVehicleStatusParameters(requestFilter, out string field);
                if (isValid)
                {
                    var response = new RfmsVehiclePosition();
                    response = await _rfmsManager.GetVehiclePosition(vehiclePositionRequest);
                    return Ok(_mapper.MapVehiclePositionResponse(response));
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

        #region Vehicle Status endpoint
        [HttpGet]
        [Route("vehiclestatus")]
        [Authorize(Policy = AccessPolicies.RFMS_VEHICLE_POSITION_ACCESS_POLICY)]
        public async Task<IActionResult> GetVehiclestatus([FromQuery] string datetype,
                                                            [FromQuery] string starttime,
                                                            [FromQuery] string stoptime,
                                                            [FromQuery] string vin,
                                                            [FromQuery] bool latestOnly,
                                                            [FromQuery] string triggerFilter,
                                                            [FromQuery] string lastVin,
                                                            [FromQuery] string contentFilter)
        {
            try
            {

                await GetUserDetails();
                var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(AccountId, OrgId);
                if (visibleVehicles.Count() == 0)
                {
                    var response = new RfmsVehicleStatus();
                    var message = string.Format(RFMSResponseTypeConstants.GET_VIN_VISIBILITY_FAILURE_MSG, AccountId, OrgId);
                    _logger.LogError(message);
                    return Ok(response);

                }

                var request = new RfmsVehiclePositionStatusFilter() { Vin = vin, LastVin = lastVin, LatestOnly = latestOnly, StartTime = starttime, StopTime = stoptime, TriggerFilter = triggerFilter, Type = datetype };

                var requestFilter = new RfmsVehicleStatusRequest() { RfmsVehicleStatusFilter = request, ContentFilter = contentFilter };
                var selectedType = ValidateHeaderRequest(requestFilter.RfmsVehicleStatusFilter, RFMSResponseTypeConstants.ACCEPT_TYPE_VEHICLE_STATUS_JSON, out bool isHeaderValid);
                if (!isHeaderValid)
                    return selectedType;





                //Get Threshold Value from Congifurations
                var thresholdRate = _configuration.GetSection("rfms3.vehiclestatus").GetSection("DataThresholdValue").Value;
                int thresholdValue = Convert.ToInt32(thresholdRate);

                RfmsVehicleStatusRequest rfmsVehicleStatusRequest = _mapper.MapVehicleStatusRequest(requestFilter.RfmsVehicleStatusFilter);
                rfmsVehicleStatusRequest.ContentFilter = string.IsNullOrEmpty(requestFilter.ContentFilter) ? string.Empty : requestFilter.ContentFilter.ToUpper();

                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Rfms Vehicle Position Service", "Rfms Vehicle Position Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get vehicle position method vehicle position service", 0, 0, JsonConvert.SerializeObject(rfmsVehicleStatusRequest), 0, 0);

                rfmsVehicleStatusRequest.OrgId = OrgId;
                rfmsVehicleStatusRequest.AccountId = AccountId;
                rfmsVehicleStatusRequest.ThresholdValue = thresholdValue;

                var isValid = _mapper.ValidateVehicleStatusParameters(requestFilter.RfmsVehicleStatusFilter, out string field);
                if (isValid)
                {
                    var response = new RfmsVehicleStatus();
                    response = await _rfmsManager.GetRfmsVehicleStatus(rfmsVehicleStatusRequest);
                    return Ok(response);
                }
                return GenerateErrorResponse(HttpStatusCode.BadRequest, field, "INVALID_PARAMETER");
            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Rfms Vehicle status data.");
                return StatusCode(500, string.Empty);
            }
        }


        #endregion

        private IActionResult ValidateHeaderRequest(RfmsVehiclePositionStatusFilter requestFilter, string acceptType, out bool isHeaderValid)
        {
            var selectedType = string.Empty;
            isHeaderValid = false;

            #region Request Header & Querystring Parameters checks
            this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

            if (!this.Request.Headers.ContainsKey("Accept") || (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept", "INVALID_PARAMETER");

            //Check if no mandatory parameters are supplied at all & raise error
            if (!requestFilter.LatestOnly && string.IsNullOrEmpty(requestFilter.StartTime))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, "latestOnly or StartTime", "INVALID_SUPPLIED_PARAMETERS, LatestOnly or StartTime parameters are mandatory to get result");

            //If latestOnly and starttime and/or stoptime are set, a HTTP 400 error will be returned indicating that the parameters supplied are invalid.
            if (requestFilter.LatestOnly && (!string.IsNullOrEmpty(requestFilter.StartTime) || !string.IsNullOrEmpty(requestFilter.StopTime)))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(requestFilter.LatestOnly), "INVALID_SUPPLIED_PARAMETERS, LatestOnly cannot be combined with StartTime and/or StopTime");

            if (acceptHeader.Any(x => x.Trim().Equals(acceptType, StringComparison.CurrentCultureIgnoreCase)))

            {
                isHeaderValid = true;
                selectedType = acceptType;
            }

            else
                return GenerateErrorResponse(HttpStatusCode.NotAcceptable, "Accept", "NOT_ACCEPTABLE value in accept - " + acceptHeader);

            return Ok(selectedType);
            #endregion
        }

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
        private async Task GetUserDetails()
        {
            var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
            var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());
            var orgs = await _accountManager.GetAccountOrg(account.Id);
            OrgId = orgs.First().Id;
            AccountId = account.Id;
        }
    }
}
