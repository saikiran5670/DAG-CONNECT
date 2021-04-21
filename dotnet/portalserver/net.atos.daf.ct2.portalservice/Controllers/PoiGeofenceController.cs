using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("poigeofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PoiGeofenceController : ControllerBase
    {
        private readonly PoiGeofenceService.PoiGeofenceServiceClient _poiGeofenceServiceClient;
        private readonly AuditHelper _auditHelper;
        public PoiGeofenceController(PoiGeofenceService.PoiGeofenceServiceClient poiGeofenceServiceClient, AuditHelper auditHelper)
        {
            _poiGeofenceServiceClient = poiGeofenceServiceClient;
            _auditHelper = auditHelper;
        }


        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get()
        {
            return Ok(0);        
        
        }


        }
}
