using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;
using net.atos.daf.ct2.poiservice;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("poi")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkPOIController : ControllerBase
    {
        private readonly POIService.POIServiceClient _poiServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public LandmarkPOIController(POIService.POIServiceClient poiServiceClient, AuditHelper auditHelper)
        {
            _poiServiceClient = poiServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Mapper();
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
                var data = await _poiServiceClient.GetAllGobalPOIAsync(objPOIEntityRequest);
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
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePOI(POI request)
        {
            try
            {
                // Validation 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The POI name is required.");
                }
                var poiRequest = new POIRequest();
                poiRequest = _mapper.ToPOIRequest(request);
                poiservice.POIResponse poiResponse = await _poiServiceClient.CreatePOIAsync(poiRequest);
                ///var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (poiResponse != null && poiResponse.Code == Responsecode.Failed)
                {
                    return StatusCode(500, "There is an error creating poi.");
                }
                else if (poiResponse != null && poiResponse.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, poiResponse.Message);
                }
                else if (poiResponse != null && poiResponse.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                    "POI service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Create method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    Request);
                    return Ok(poiResponse);
                }
                else
                {
                    return StatusCode(404, "POI Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                 "POI service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  Request);
                //_logger.Error(null, ex);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdatePOI(POI request)
        {
            try
            {
                // Validation 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The POI name is required.");
                }
                var poiRequest = new POIRequest();
                poiRequest = _mapper.ToPOIRequest(request);
                poiservice.POIResponse poiResponse = await _poiServiceClient.UpdatePOIAsync(poiRequest);

                if (poiResponse != null && poiResponse.Code == Responsecode.Failed)
                {
                    return StatusCode(500, "There is an error creating poi.");
                }
                else if (poiResponse != null && poiResponse.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                    "POI service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Update method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    Request);
                    return Ok(poiResponse);
                }
                else
                {
                    return StatusCode(404, "Geofence Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                 "POI service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPut]
        [Route("delete")]
        public async Task<IActionResult> DeletePOI(POI request)
        {
            try
            {
                // Validation 
                if (request.Id==0)
                {
                    return StatusCode(400, "The POI id is required.");
                }
                var poiRequest = new POIRequest();
                poiRequest.Id = request.Id;
                poiservice.POIResponse poiResponse = await _poiServiceClient.DeletePOIAsync(poiRequest);

                if (poiResponse != null && poiResponse.Code == Responsecode.Failed)
                {
                    return StatusCode(500, "There is an error creating poi.");
                }
                else if (poiResponse != null && poiResponse.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                    "POI service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Delete method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    Request);
                    return Ok(poiResponse);
                }
                else
                {
                    return StatusCode(404, "Geofence Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                 "POI service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Delete method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
