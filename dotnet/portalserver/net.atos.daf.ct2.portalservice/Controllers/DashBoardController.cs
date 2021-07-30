﻿using System;
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
        private readonly AuditHelper _auditHelper;


        public DashBoardController(DashboardService.DashboardService.DashboardServiceClient dashboardClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _dashboarClient = dashboardClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditHelper = auditHelper;
        }

        [HttpGet]
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

    }
}
