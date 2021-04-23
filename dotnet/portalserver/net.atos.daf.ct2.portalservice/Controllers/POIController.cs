using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;
using net.atos.daf.ct2.poiservice;


namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("poi")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandMarkPOIController : ControllerBase
    {
        private readonly POIService.POIServiceClient _poiGeofenceServiceClient;
        private readonly AuditHelper _auditHelper;
        //private readonly Mapper _mapper;
        public LandMarkPOIController(POIService.POIServiceClient poiGeofenceServiceClient, AuditHelper auditHelper)
        {
            _poiGeofenceServiceClient = poiGeofenceServiceClient;
            _auditHelper = auditHelper;
            //_mapper = new Mapper();
        }


      
        [HttpGet]
        [Route("getallglobalpoi")]
        public async Task<IActionResult> getallglobalpoi([FromQuery] net.atos.daf.ct2.portalservice.Entity.POI.POIEntityRequest request)
        {
            try
            {
                //_logger.Info("Get method in vehicle API called.");
                net.atos.daf.ct2.poiservice.POIEntityRequest objPOIEntityRequest = new net.atos.daf.ct2.poiservice.POIEntityRequest();
                if (request.organization_id <= 0)
                {
                    return StatusCode(400, string.Empty);
                }
                //objPOIEntityRequest.OrganizationId = request.organization_id;
                var data = await _poiGeofenceServiceClient.GetAllGobalPOIAsync(objPOIEntityRequest);
                if (data != null)
                {
                    if (data.POIList != null && data.POIList.Count > 0)
                    {
                        return Ok(data.POIList);
                    }
                    else
                    {
                        return StatusCode(404, string.Empty);
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
