using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.alertservice;
using net.atos.daf.ct2.portalservice.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("alert")]
    public class AlertController : ControllerBase
    {
        private ILog _logger;
        private readonly AlertService.AlertServiceClient _AlertServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly HeaderObj _userDetails;

        public AlertController(AlertService.AlertServiceClient AlertServiceClient, AuditHelper auditHelper, Common.AccountPrivilegeChecker privilegeChecker,IHttpContextAccessor _httpContextAccessor)
        {
            _AlertServiceClient = AlertServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }
    }
}
