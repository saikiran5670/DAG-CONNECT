using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.dashboardservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Dashboard;
using net.atos.daf.ct2.portalservice.Entity.Report;
using Newtonsoft.Json;
using DashboardService = net.atos.daf.ct2.dashboardservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("dashboard")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DashBoardController : BaseController
    {

        private readonly ILog _logger;
        private readonly DashboardService.DashboardService.DashboardServiceClient _dashboardServiceClient;
        private readonly string _socketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly AuditHelper _auditHelper;
        private readonly DashboardMapper _dashboardMapper;



        public DashBoardController(DashboardService.DashboardService.DashboardServiceClient dashboardClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _dashboardServiceClient = dashboardClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditHelper = auditHelper;
            _dashboardMapper = new DashboardMapper();
        }

        [HttpPost]
        [Route("fleetkpi")]
        public async Task<IActionResult> GetFleetKpi([FromBody] Entity.Dashboard.DashboardFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetKpiFilterRequest objDashboardFilter = JsonConvert.DeserializeObject<FleetKpiFilterRequest>(filters);
                _logger.Info("GetFleetKpi method in dashboard API called.");
                var data = await _dashboardServiceClient.GetFleetKPIDetailsAsync(objDashboardFilter);
                if (data?.FleetKpis != null)
                {
                    data.Message = DashboardConstant.GET_DASBHOARD_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, DashboardConstant.GET_DASBHOARD_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetKpi)}: With Error:-", ex);
                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }
        }

        //[HttpPost]
        [HttpGet]
        [Route("alert24hours")]
        public async Task<IActionResult> GetAlert24Hours()
        {
            try
            {
                var featureIds = GetMappedFeatureIdByStartWithName(DashboardConstant.ALERT_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                int organizationId = GetContextOrgId();
                headers.Add("context_orgid", Convert.ToString(organizationId));
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("logged_in_accId", Convert.ToString(_userDetails.AccountId));

                Alert24HoursFilterRequest objAlertFilter = new Alert24HoursFilterRequest();
                _logger.Info("GetAlert24hours method in dashboard API called.");
                var data = await _dashboardServiceClient.GetLastAlert24HoursAsync(objAlertFilter, headers);
                if (data != null)
                {
                    data.Message = data.Code == Responsecode.Success ? DashboardConstant.GET_ALERTLAST24HOURS_SUCCESS_MSG : data.Message;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, DashboardConstant.GET_ALERTLAST24HOURS_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAlert24Hours)}: With Error:-", ex);
                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }

        }

        #region Fleetutilization
        [HttpPost]
        [Route("fleetutilization")]
        public async Task<IActionResult> GetFleetutilization([FromBody] Entity.Dashboard.DashboardFilter request)
        {
            try
            {
                if (request == null && !(request.StartDateTime > 0)) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(DashboardConstant.GET_DASBHOARD_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetKpiFilterRequest objDashboardFilter = JsonConvert.DeserializeObject<FleetKpiFilterRequest>(filters);
                _logger.Info("GetFleetKpi method in dashboard API called.");
                var data = await _dashboardServiceClient.GetFleetUtilizationDetailsAsync(objDashboardFilter);
                if (data != null)
                {
                    data.Message = DashboardConstant.GET_DASBHOARD_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, DashboardConstant.GET_DASBHOARD_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetutilization)}: With Error:-", ex);
                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        [HttpPost]
        [Route("todaylive")]
        public async Task<IActionResult> GetTodayLiveVinData([FromBody] Entity.Dashboard.TodayLiveVehicleRequest request)
        {
            try
            {
                if (request == null && request.VINs.Count <= 0)
                {
                    return BadRequest(DashboardConstant.GET_ALERTLAST24HOURS_VALIDATION_VINREQUIRED_MSG);
                }
                string filters = JsonConvert.SerializeObject(request);
                _logger.Info("GetTodayLiveVinData method in dashboard API called.");
                var data = await _dashboardServiceClient.GetTodayLiveVinDataAsync(JsonConvert.DeserializeObject<dashboardservice.TodayLiveVehicleRequest>(filters));
                switch (data.Code)
                {
                    case Responsecode.Success:
                        return Ok(data);
                    case Responsecode.Failed:
                        return StatusCode((int)data.Code, data);
                    case Responsecode.InternalServerError:
                        return StatusCode(500, "Internal Server Error.");
                    default:
                        return StatusCode((int)data.Code, data.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetTodayLiveVinData)}: With Error:-", ex);
                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("vins")]
        public async Task<IActionResult> GetVisibleVins(int accountId, int organizationId)
        {
            try
            {
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                organizationId = GetContextOrgId();
                if (!(accountId > 0)) return BadRequest(DashboardConstant.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(DashboardConstant.ORGANIZATION_REQUIRED_MSG);

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));

                var response = await _dashboardServiceClient.GetVisibleVinsAsync(
                                              new VehicleListRequest { AccountId = accountId, OrganizationId = organizationId }, headers);

                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.Failed)
                    return StatusCode((int)response.Code, response);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(DashboardConstant.GET_VIN_VISIBILITY_FAILURE_MSG2, accountId, organizationId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetVinsFromTripStatisticsAndVehicleDetails method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                _logger.Error($"{nameof(GetVisibleVins)}: With Error:-", ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("preference")]
        public async Task<IActionResult> GetDashboardUserPreference(int reportId)
        {
            try
            {
                if (reportId < 1) return BadRequest(ReportConstants.REPORT_REQUIRED_MSG);

                DashboardUserPreferenceRequest userPrefRequest = new DashboardUserPreferenceRequest();
                userPrefRequest.ReportId = reportId;
                userPrefRequest.AccountId = _userDetails.AccountId;
                userPrefRequest.RoleId = _userDetails.RoleId;
                userPrefRequest.OrganizationId = GetUserSelectedOrgId();
                userPrefRequest.ContextOrgId = GetContextOrgId();

                var subReportResponse = await _dashboardServiceClient.CheckIfSubReportExistAsync(new CheckIfSubReportExistRequest { ReportId = reportId });

                // Send sub report features from session to find attributes of related reports
                SessionFeatures[] objUserFeatures;
                if (subReportResponse != null)
                {
                    var sessionFeatures = GetUserSubscribeFeatures();
                    if (subReportResponse.HasSubReports == "Y" && subReportResponse.FeatureId > 0)
                    {
                        var featureName = sessionFeatures?.Where(x => x.FeatureId == subReportResponse.FeatureId)?.Select(x => x.Name)?.FirstOrDefault();

                        if (!string.IsNullOrEmpty(featureName))
                        {
                            var logbookFeatureToExclude = sessionFeatures.Where(x => x.Name.Equals("FleetOverview.LogBook"));
                            var requiredFeatures = sessionFeatures.Where(x => x.Name.StartsWith(featureName)).Except(logbookFeatureToExclude);

                            if (requiredFeatures.Count() > 0)
                            {
                                string strFeature = JsonConvert.SerializeObject(requiredFeatures);
                                objUserFeatures = JsonConvert.DeserializeObject<SessionFeatures[]>(strFeature);
                                if (objUserFeatures != null) { userPrefRequest.UserFeatures.AddRange(objUserFeatures); }
                            }
                        }
                    }
                    else if (subReportResponse.HasSubReports == "N" && subReportResponse.FeatureId > 0)
                    {
                        if (!sessionFeatures.Any(x => x.FeatureId == subReportResponse.FeatureId))
                            return StatusCode(404, "No data found.");
                    }
                }

                var response = await _dashboardServiceClient.GetDashboardUserPreferenceAsync(userPrefRequest);
                if (response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Dashboard Controller", "Dashboard service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     $"{ nameof(GetDashboardUserPreference) } method", 1, 2, Convert.ToString(reportId), _userDetails);

                    return Ok(new { TargetProfileId = response.TargetProfileId, UserPreferences = response.UserPreference });
                }
                if (response.Code == Responsecode.InternalServerError)
                { return StatusCode((int)response.Code, string.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, response.Message)); }
                else
                { return StatusCode((int)response.Code, response.Message); }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Dashboard Controller", "Dashboard service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"{ nameof(GetDashboardUserPreference) } method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId), _userDetails);

                _logger.Error($"{nameof(GetDashboardUserPreference)}: With Error:-", ex);

                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreateDashboardUserPreference(Entity.Dashboard.DashboardUserPreferenceCreateRequest objDashUserPreferenceCreateRequest)
        {
            try
            {
                var request = _dashboardMapper.MapCreateDashboardUserPreference(objDashUserPreferenceCreateRequest, _userDetails.AccountId, GetContextOrgId());
                var responsed = await _dashboardServiceClient.CreateDashboardUserPreferenceAsync(request);

                if (responsed.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Dashboard Controller",
                            "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS, "Report use preference created successfully", 0, 0, JsonConvert.SerializeObject(objDashUserPreferenceCreateRequest),
                                _userDetails);
                    return Ok(responsed.Message);
                }
                else
                {
                    return StatusCode((int)responsed.Code, responsed.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Dashboard Controller",
                                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                 $"{ nameof(CreateDashboardUserPreference) } method Failed. Error : {ex.Message}", 0, 0, JsonConvert.SerializeObject(objDashUserPreferenceCreateRequest),
                                  _userDetails);
                _logger.Error($"{nameof(CreateDashboardUserPreference)}: With Error:-", ex);
                return StatusCode(500, DashboardConstant.INTERNAL_SERVER_MSG);
            }
        }
    }
}
