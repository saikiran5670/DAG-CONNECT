using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.reportschedulerservice;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("reportscheduler")]
    public class ReportSchedulerController : BaseController
    {
        private ILog _logger;
        private readonly ReportSchedulerService.ReportSchedulerServiceClient _reportschedulerClient;
        private readonly AuditHelper _auditHelper;
        public ReportSchedulerController(ReportSchedulerService.ReportSchedulerServiceClient reportschedulerClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _reportschedulerClient = reportschedulerClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
