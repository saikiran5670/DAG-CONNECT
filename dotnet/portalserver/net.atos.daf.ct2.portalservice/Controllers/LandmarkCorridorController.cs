using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.portalservice.Common;
using System.Reflection;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("corridor")]
    public class LandmarkCorridorController : ControllerBase
    {

        private ILog _logger;
        private readonly CorridorService.CorridorServiceClient _corridorServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.POI.Mapper _mapper;
        private readonly OrganizationService.OrganizationServiceClient _organizationClient;
        private readonly HeaderObj _userDetails;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public LandmarkCorridorController(CorridorService.CorridorServiceClient corridorServiceClient, AuditHelper auditHelper,
            Common.AccountPrivilegeChecker privilegeChecker, IHttpContextAccessor _httpContextAccessor)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorServiceClient = corridorServiceClient;
            _auditHelper = auditHelper;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }
    }

}
