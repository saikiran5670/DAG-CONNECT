﻿using Microsoft.AspNetCore.Authentication.Cookies;
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
using net.atos.daf.ct2.organizationservice;
using Microsoft.AspNetCore.Http;

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
        private readonly OrganizationService.OrganizationServiceClient _organizationClient;
        private readonly HeaderObj _userDetails;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public LandmarkPOIController(POIService.POIServiceClient poiServiceClient, AuditHelper auditHelper, 
            Common.AccountPrivilegeChecker privilegeChecker, IHttpContextAccessor _httpContextAccessor)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _poiServiceClient = poiServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Entity.POI.Mapper();
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
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
                if (request.OrganizationId <= 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global poi.");
                }
                var poiRequest = new POIRequest();
                request.State= "Active";
                poiRequest = _mapper.ToPOIRequest(request);
                poiservice.POIResponse poiResponse = await _poiServiceClient.CreatePOIAsync(poiRequest);

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
                
                if (request.OrganizationId <= 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global poi.");
                }

                // Validation 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The POI name is required.");
                }
                if (request.Id <= 0)
                {
                    return StatusCode(400, "The POI Id is required.");
                }

                var poiRequest = new POIRequest();                
                poiRequest = _mapper.ToPOIRequest(request);
                poiservice.POIResponse poiResponse = await _poiServiceClient.UpdatePOIAsync(poiRequest);

                if (poiResponse != null && poiResponse.Code == Responsecode.Failed)
                {
                    return StatusCode(500, "There is an error updating poi.");
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
                    return StatusCode(404, "POI Response is null");
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
        //[HttpDelete]
        //[Route("delete")]
        //public async Task<IActionResult> DeletePOI(int Id)
        //{
        //    try
        //    {
        //        if (Id==0)
        //        {
        //            return StatusCode(400, "The POI id is required.");
        //        }
        //        var poiRequest = new POIRequest();
        //        poiRequest.Id = Id;
        //        poiservice.POIResponse poiResponse = await _poiServiceClient.DeletePOIAsync(poiRequest);

        //        if (poiResponse != null && poiResponse.Code == Responsecode.Failed)
        //        {
        //            return StatusCode(500, "There is an error deleting poi.");
        //        }
        //        else if (poiResponse != null && poiResponse.Code == Responsecode.Success)
        //        {
        //            await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
        //            "POI service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
        //            "Delete method in POI controller", Id, Id, JsonConvert.SerializeObject(Id),
        //            Request);
        //            return Ok("POI has been deleted" + Id);
        //        }
        //        else
        //        {
        //            return StatusCode(404, "POI Response is null");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
        //         "POI service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
        //         "Delete method in POI controller", Id, Id, JsonConvert.SerializeObject(Id),
        //          Request);
        //        // check for fk violation
        //        if (ex.Message.Contains(SocketException))
        //        {
        //            return StatusCode(500, "Internal Server Error.(02)");
        //        }
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeletePOIBulk(List<int> ids)
        {
            try
            {
                if (ids.Count==0)
                {
                    return StatusCode(400, "The POI id is required.");
                }
                POIDeleteBulkRequest bulkRequest = new POIDeleteBulkRequest();
                foreach (var item in ids)
                {
                    bulkRequest.Id.Add(item);
                }
                poiservice.POIResponse poiResponse = await _poiServiceClient.DeletePOIBulkAsync(bulkRequest);

                if (poiResponse != null && poiResponse.Code == Responsecode.Failed)
                {
                    return StatusCode(500, "There is an error deleting poi.");
                }
                else if (poiResponse != null && poiResponse.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                    "POI service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "DeletePOIBulk method in POI controller", 0, 0, JsonConvert.SerializeObject(ids),
                    Request);
                    poiResponse.Message = "POI's has been deleted";                   
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
                 "POI service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "DeletePOIBulk method in POI controller", 0, 0, JsonConvert.SerializeObject(ids),
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
                if (OrganizationId <= 0)
                {
                    return StatusCode(400, "OrganizationId data is required.");
                }
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

        [HttpPost]
        [Route("uploadexcel")]
        public async Task<IActionResult> UploadExcel(List<POI> request)
        {
            try
            {
                // Validation 
                if (request.Count <= 0)
                {
                    return StatusCode(400, "poi data is required.");
                }
                var poiUploadRequest =_mapper.ToUploadRequest(request);
              
                var poiUploadResponse = await _poiServiceClient.UploadPOIExcelAsync(poiUploadRequest);
               

                if (poiUploadResponse != null && poiUploadResponse.Code == Responsecode.Failed)
                {
                    return StatusCode(500, "There is an error creating poi.");
                }
                else if (poiUploadResponse != null && poiUploadResponse.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, poiUploadResponse.Message);
                }
                else if (poiUploadResponse != null && poiUploadResponse.Code == Responsecode.Success)
                {
                    //await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                    //"POI service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    //"Create method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    //Request);
                    return Ok(poiUploadResponse);
                }
                else
                {
                    return StatusCode(404, "POI Response is null");
                }

            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "POI Component",
                // "POI service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // "Create  method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                //  Request);
                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }               
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [NonAction]
        public async Task<bool> HasAdminPrivilege()
        {
            bool Result = false;
            try
            {
                int level = await _privilegeChecker.GetLevelByRoleId(_userDetails.orgId, _userDetails.roleId);
                if (level == 10 || level == 20)
                    Result = true;
                else
                    Result = false;
            }
            catch (Exception)
            {
                Result = false;
            }
            return Result;
        }

        [HttpGet]
        [Route("getalltripdetails")]
        public async Task<IActionResult> GetAllTripDetails([FromQuery] net.atos.daf.ct2.portalservice.Entity.POI.TripEntityRequest request)
        {
            try
            {
                _logger.Info("GetAllTripDetails method in POI API called.");
                TripRequest objTripRequest = new TripRequest();
                objTripRequest.VIN = request.VIN;
                objTripRequest.StartDateTime = request.StartDateTime;
                objTripRequest.EndDateTime = request.EndDateTime;                
                var data = await _poiServiceClient.GetAllTripDetailsAsync(objTripRequest);
                if (data != null )
                {
                  return Ok(data);                   
                }
                else
                {
                    return StatusCode(404, "Trip details are not found");
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

