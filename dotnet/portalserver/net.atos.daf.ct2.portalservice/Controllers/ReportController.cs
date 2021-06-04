using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Report;
using net.atos.daf.ct2.reportservice;
using Newtonsoft.Json;
using static net.atos.daf.ct2.reportservice.ReportService;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("report")]
    public class ReportController : BaseController
    {
        private ILog _logger;
        private readonly ReportServiceClient _reportServiceClient;
        private readonly AuditHelper _auditHelper;
        private string _socketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly Mapper _mapper;

        public ReportController(ReportServiceClient reportServiceClient, AuditHelper auditHelper,
                               IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _reportServiceClient = reportServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
        }

        #region Select User Preferences

        [HttpGet]
        [Route("getreportdetails")]
        public async Task<IActionResult> GetReportDetails()
        {
            try
            {
                var response = await _reportServiceClient.GetReportDetailsAsync(new TempPara { TempID = 0 });
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.GET_REPORT_DETAILS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetUserPreferenceReportDataColumn method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getuserpreferencereportdatacolumn")]
        public async Task<IActionResult> GetUserPreferenceReportDataColumn(int reportId, int accountId, int organizationId)
        {
            try
            {
                if (!(reportId > 0)) return BadRequest(ReportConstants.REPORT_REQUIRED_MSG);
                if (!(accountId > 0)) return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG);
                var response = await _reportServiceClient.GetUserPreferenceReportDataColumnAsync(new IdRequest { ReportId = reportId, AccountId = accountId, OrganizationId = organizationId });
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.Failed)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, ReportConstants.USER_PREFERENCE_FAILURE_MSG2));
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetUserPreferenceReportDataColumn method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion


        #region - Trip Report Table Details
        [HttpGet]
        [Route("gettripdetails")]
        public async Task<IActionResult> GetFilteredTripDetails([FromQuery] TripFilterRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_STARTDATE_MSG);
                if (!(request.EndDateTime > 0)) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_ENDDATE_MSG);
                if (string.IsNullOrEmpty(request.VIN)) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_VINREQUIRED_MSG);
                if (request.StartDateTime > request.EndDateTime) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG);

                _logger.Info("GetFilteredTripDetailsAsync method in Report (Trip Report) API called.");
                var data = await _reportServiceClient.GetFilteredTripDetailsAsync(request);
                if (data?.TripData?.Count > 0)
                {
                    data.Message = ReportConstants.GET_TRIP_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_TRIP_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        [HttpPost]
        [Route("createuserpreference")]
        public async Task<IActionResult> CreateUserPreference(net.atos.daf.ct2.portalservice.Entity.Report.UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            try
            {
                var request = _mapper.MapCreateUserPrefences(objUserPreferenceCreateRequest);
                var response = await _reportServiceClient.CreateUserPreferenceAsync(request);
                if (response == null)
                    return StatusCode(500, "Internal Server Error.");

                switch (response.Code)
                {
                    case Responsecode.Success:
                        await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS, "Report preference created successfully", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                 _userDetails);
                        return Ok(response);
                    case Responsecode.Failed:
                        return StatusCode((int)response.Code, response.Message);
                    case Responsecode.InternalServerError:
                        return StatusCode((int)response.Code, response.Message);
                    default:
                        return StatusCode((int)response.Code, response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                 $"createuserpreference method Failed. Error:{ex.Message}", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message} {ex.StackTrace}");
            }
        }

        #region Select User Preferences
        [HttpGet]
        [Route("getvinsfromtripstatisticsandvehicledetails")]
        public async Task<IActionResult> GetVinsFromTripStatisticsAndVehicleDetails(int accountId, int organizationId)
        {
            try
            {
                if (!(accountId > 0)) return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG);
                var response = await _reportServiceClient
                                            .GetVinsFromTripStatisticsWithVehicleDetailsAsync
                                            (
                                              new VehicleListRequest { AccountId = accountId, OrganizationId = organizationId }
                                            );

                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.Failed)
                    return StatusCode((int)response.Code, response);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG2, accountId, organizationId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetVinsFromTripStatisticsAndVehicleDetails method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                _logger.Error(null, ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region - Driver Time Management Report Table Details
        [HttpPost]
        [Route("getdriverstimedetails")]
        public async Task<IActionResult> GetDriversActivity([FromBody] ActivityFilterRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (string.IsNullOrEmpty(request.VINs)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (string.IsNullOrEmpty(request.DriverIds)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG); }

                _logger.Info("GetDriversActivityAsync method in Report (Multiple Driver Time details Report) API called.");
                var data = await _reportServiceClient.GetDriversActivityAsync(request);
                if (data?.DriverActivities?.Count > 0)
                {
                    data.Message = ReportConstants.GET_TRIP_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_TRIP_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("getsingledrivertimedetails")]
        public async Task<IActionResult> GetDriverActivity([FromBody] ActivityFilterRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (string.IsNullOrEmpty(request.VINs)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (string.IsNullOrEmpty(request.DriverIds)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG); }

                _logger.Info("GetDriverActivityAsync method in Report (Single Driver Time details Report) API called.");
                var data = await _reportServiceClient.GetDriverActivityAsync(request);
                if (data?.DriverActivities?.Count > 0)
                {
                    data.Message = ReportConstants.GET_DRIVER_TIME_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_DRIVER_TIME_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("getdriveractivityparameters")]
        public async Task<IActionResult> GetDriverActivityParameters([FromBody] IdRequestForDriverActivity request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (!(request.OrganizationId > 0)) { return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG); }
                if (!(request.AccountId > 0)) { return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG); }

                _logger.Info("GetDriverActivityParameters method in Report API called.");
                var data = await _reportServiceClient.GetDriverActivityParametersAsync(request);
                if (data?.VehicleDetailsWithAccountVisibiltyList?.Count > 0)
                {
                    data.Message = ReportConstants.GET_DRIVER_TIME_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_DRIVER_TIME_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion
    }
}
