using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;

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

        [HttpGet]
        [Route("getallpoi")]
        public async Task<IActionResult> GetAllPOI([FromQuery]net.atos.daf.ct2.portalservice.Entity.POI.POIEntityRequest request)
        {
            try
            {
                //_logger.Info("Get method in vehicle API called.");
                net.atos.daf.ct2.poigeofenceservice.POIEntityRequest objPOIEntityRequest = new net.atos.daf.ct2.poigeofenceservice.POIEntityRequest();
                if (request.organization_id <= 0)
                {
                    return StatusCode(400, string.Empty);
                }
                objPOIEntityRequest.OrganizationId = request.organization_id;
                var data = await _poiGeofenceServiceClient.GetAllPOIAsync(objPOIEntityRequest);
                if (data != null )
                {
                    if (data.POIList != null && data.POIList.Count > 0)
                    {
                        return Ok(data.POIList);
                    }
                    else
                    {
                        return StatusCode(404,string.Empty);
                    }
                }
                else
                {
                    return StatusCode(500, string.Empty);
                }

            }

            catch (Exception ex)
            {
                //_logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }
}
