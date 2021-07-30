using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.subscription.entity;
using Newtonsoft.Json;
using DashboardService = net.atos.daf.ct2.dashboardservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("dashboard")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DashBoardController : BaseController
    {
        #region Private Variable

        private readonly ILog _logger;
        private readonly DashboardService.DashboardService.DashboardServiceClient _subscribeClient;
        private readonly AuditHelper _auditHelper;

        #endregion

        #region Constructor
        public DashBoardController(DashboardService.DashboardService.DashboardServiceClient subscribeClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _subscribeClient = subscribeClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditHelper = auditHelper;
        }
        #endregion


        [HttpGet]
        [Route("fleetkpi")]
        public async Task<IActionResult> GetFleetKpi()
        {
            throw NotImplementedException();
        }

        private Exception NotImplementedException() => throw new NotImplementedException();
    }
}
