using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using System.Reflection;
using static net.atos.daf.ct2.reportservice.ReportService;
using Report = net.atos.daf.ct2.portalservice.Entity.Report;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private ILog _logger;
        private readonly ReportServiceClient _reportServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly HeaderObj _userDetails;
        private readonly Report.Mapper _mapper;

        public ReportController(ReportServiceClient reportServiceClient,
                               AuditHelper auditHelper,
                               Common.AccountPrivilegeChecker privilegeChecker,
                               IHttpContextAccessor httpContextAccessor)
        {
            _reportServiceClient = reportServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(httpContextAccessor.HttpContext.Request);
            _mapper = new Report.Mapper();
        }

        
    }
}
