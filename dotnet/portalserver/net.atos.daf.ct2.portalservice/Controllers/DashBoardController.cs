using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.dashboardservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Dashboard;
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
        private readonly DashboardService.DashboardService.DashboardServiceClient _dashboarClient;
        private readonly string _socketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly AuditHelper _auditHelper;


        public DashBoardController(DashboardService.DashboardService.DashboardServiceClient dashboardClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _dashboarClient = dashboardClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditHelper = auditHelper;
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
                var data = await _dashboarClient.GetFleetKPIDetailsAsync(objDashboardFilter);
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
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("alert24hours")]
        public async Task<IActionResult> GetAlert24Hours([FromBody] Entity.Dashboard.Alert24HoursFilter request)
        {
            try
            {

                if (request.VINs.Count <= 0)
                {
                    return BadRequest(DashboardConstant.GET_ALERTLAST24HOURS_VALIDATION_VINREQUIRED_MSG);
                }
                string filters = JsonConvert.SerializeObject(request);
                Alert24HoursFilterRequest objAlertFilter = JsonConvert.DeserializeObject<Alert24HoursFilterRequest>(filters);
                _logger.Info("GetAlert24hours method in dashboard API called.");
                var data = await _dashboarClient.GetLastAlert24HoursAsync(objAlertFilter);
                if (data != null)
                {
                    data.Message = DashboardConstant.GET_ALERTLAST24HOURS_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, DashboardConstant.GET_ALERTLAST24HOURS_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }

        }

        #region Fleetutilization
        [HttpPost]
        [Route("fleetutilization")]
        public async Task<IActionResult> GetFleetutilization([FromBody] Entity.Dashboard.DashboardFilter request)
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
                var data = await _dashboarClient.GetFleetUtilizationDetailsAsync(objDashboardFilter);
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
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        [HttpPost]
        [Route("todaylive")]
        public async Task<IActionResult> GetTodayLiveVinData([FromBody] Entity.Dashboard.TodayLiveVehicleRequest request)
        {
            try
            {
                if (request.VINs.Count <= 0)
                {
                    return BadRequest(DashboardConstant.GET_ALERTLAST24HOURS_VALIDATION_VINREQUIRED_MSG);
                }
                string filters = JsonConvert.SerializeObject(request);
                _logger.Info("GetTodayLiveVinData method in dashboard API called.");
                var data = await _dashboarClient.GetTodayLiveVinDataAsync(JsonConvert.DeserializeObject<dashboardservice.TodayLiveVehicleRequest>(filters));
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, string.Format("{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        [HttpGet]
        [Route("vins")]
        public async Task<IActionResult> GetVisibleVins(int accountId, int organizationId)
        {
            try
            {
                if (!(accountId > 0)) return BadRequest(DashboardConstant.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(DashboardConstant.ORGANIZATION_REQUIRED_MSG);
                var response = await _dashboarClient.GetVisibleVinsAsync(
                                              new VehicleListRequest { AccountId = accountId, OrganizationId = organizationId });

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
                _logger.Error(null, ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
