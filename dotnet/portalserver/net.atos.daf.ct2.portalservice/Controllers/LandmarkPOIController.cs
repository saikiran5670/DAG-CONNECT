using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;
using net.atos.daf.ct2.poiservice;
using Newtonsoft.Json;
using log4net;
using System.Reflection;
using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("poi")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkPOIController : ControllerBase
    {
        private ILog _logger;
        private readonly POIService.POIServiceClient _poiServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.POI.Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public LandmarkPOIController(POIService.POIServiceClient poiServiceClient, AuditHelper auditHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _poiServiceClient = poiServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Entity.POI.Mapper();
        }

        [HttpGet]
        [Route("getallglobalpoi")]
        public async Task<IActionResult> getallglobalpoi([FromQuery] net.atos.daf.ct2.portalservice.Entity.POI.POIEntityRequest request)
        {
            try
            {
                _logger.Info("GetAllGlobalPOI method in POI API called.");
                net.atos.daf.ct2.poiservice.POIEntityRequest objPOIEntityRequest = new net.atos.daf.ct2.poiservice.POIEntityRequest();
                objPOIEntityRequest.CategoryId = request.CategoryId;//non mandatory field
                objPOIEntityRequest.SubCategoryId = request.SubCategoryId;////non mandatory field
                var data = await _poiServiceClient.GetAllGobalPOIAsync(objPOIEntityRequest);
                if (data != null && data.Code == net.atos.daf.ct2.poiservice.Responsecode.Success)
                {
                    if (data.POIList != null && data.POIList.Count > 0)
                    {
                        return Ok(data.POIList);
                    }
                    else
                    {
                        return StatusCode(404, "Global POI details are not found");
                    }
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message} {ex.StackTrace}");
            }
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePOI(POI request)
        {
            try
            {
                // Validation 
                
                var poiRequest = new POIRequest();
                request.State= "Active";
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
        [HttpGet]
        [Route("downloadpoiforexcel")]
        public async Task<IActionResult> DownLoadPOIForExcel([FromQuery] int OrganizationId)
        {
            try
            {
                _logger.Info("DownLoadPOIForExcel method in POI API called.");
                net.atos.daf.ct2.poiservice.DownloadPOIRequest objPOIEntityRequest = new net.atos.daf.ct2.poiservice.DownloadPOIRequest();
                objPOIEntityRequest.OrganizationId = OrganizationId;
                var data = await _poiServiceClient.DownloadPOIForExcelAsync(objPOIEntityRequest);
                if (data != null && data.Code == net.atos.daf.ct2.poiservice.Responsecode.Success)
                {
                    if (data.POIList != null && data.POIList.Count > 0)
                    {
                        return Ok(data.POIList);
                    }
                    else
                    {
                        return StatusCode(404, "POI details for Excel download are not found");
                    }
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message} {ex.StackTrace}");
            }
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetPOIs([FromQuery] POIFilter poiFilter)
        {
            try
            {
                _logger.Info("GetPOIs method in POI API called.");
                POIRequest poiRequest = new POIRequest();
                poiRequest.Id = poiFilter.Id;
                poiRequest.OrganizationId = poiFilter.OrganizationId;
                poiRequest.CategoryId = poiFilter.CategoryId;
                poiRequest.SubCategoryId = poiFilter.SubCategoryId;
                poiRequest.Type = "POI";
                var data = await _poiServiceClient.GetAllPOIAsync(poiRequest);
                if (data != null && data.Code == net.atos.daf.ct2.poiservice.Responsecode.Success)
                {
                    if (data.POIList != null && data.POIList.Count > 0)
                    {
                        List<net.atos.daf.ct2.portalservice.Entity.POI.POIResponse> list = new List<net.atos.daf.ct2.portalservice.Entity.POI.POIResponse>();
                        foreach (var item in data.POIList)
                        {
                            list.Add(_mapper.ToPOIEntity(item));    
                        }
                        return Ok(list);
                    }
                    else
                    {
                        return StatusCode(404, "POI details are not found");
                    }
                }
                else
                {
                    return StatusCode(500, data.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message} {ex.StackTrace}");
            }
        }
    }
}

