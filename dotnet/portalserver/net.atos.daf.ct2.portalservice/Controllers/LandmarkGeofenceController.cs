using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;
using net.atos.daf.ct2.portalservice.Entity.Geofence;
using net.atos.daf.ct2.geofenceservice;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Alert=net.atos.daf.ct2.alertservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("geofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkGeofenceController : BaseController
    {
        private ILog _logger;
        private readonly GeofenceService.GeofenceServiceClient _GeofenceServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.Geofence.Mapper _mapper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly HeaderObj _userDetails;
        private readonly Alert.AlertService.AlertServiceClient _alertServiceClient;
        public LandmarkGeofenceController(GeofenceService.GeofenceServiceClient GeofenceServiceClient, AuditHelper auditHelper,Common.AccountPrivilegeChecker privilegeChecker
            , IHttpContextAccessor _httpContextAccessor, Alert.AlertService.AlertServiceClient alertServiceClient, SessionHelper sessionHelper) : base(_httpContextAccessor, sessionHelper)
        {
            _GeofenceServiceClient = GeofenceServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Entity.Geofence.Mapper();
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
             _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
            _alertServiceClient = alertServiceClient;

        }

        #region Geofence

        [HttpPost]
        [Route("createpolygongeofence")]
        public async Task<IActionResult> CreatePolygonGeofence(Geofence request)
        {
            try
            {
                 _logger.Info("CreatePolygonGeofence method in Geofence API called.");
                if (request.OrganizationId == 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global geofence.");
                }
                else
                {
                    request.OrganizationId = GetContextOrgId();
                }
                // Validation 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The Geofence name is required.");
                }
                var geofenceRequest = new geofenceservice.GeofenceRequest();
                geofenceRequest = _mapper.ToGeofenceRequest(request);
                geofenceservice.GeofenceResponse geofenceResponse = await _GeofenceServiceClient.CreatePolygonGeofenceAsync(geofenceRequest);
                ///var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Failed)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Conflict)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Create polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   Request);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  Request);
                 _logger.Error(null, ex);
                // check for fk violation
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

        [HttpPost]
        [Route("createcirculargeofence")]
        public async Task<IActionResult> CreateCircularGeofence(List<CircularGeofence> request)
        {
            _logger.Info("CreateCircularGeofence method in Geofence API called.");
            try
            {
                bool allEqual = !request.GroupBy(o => o.OrganizationId).Skip(1).Any();
                if (allEqual)
                {
                    if (request[0].OrganizationId == 0)
                    {
                        bool hasRights = await HasAdminPrivilege();
                        if (!hasRights)
                            return StatusCode(400, "You cannot create global geofence.");
                    }
                    else
                    {
                        request[0].OrganizationId = GetContextOrgId();
                    }
                }
                else
                {
                    return StatusCode(400, "Different organization id in passed circular geofence request..");
                }
                var geofenceRequest = new geofenceservice.CircularGeofenceRequest();
                foreach (var item in request)
                {
                    geofenceRequest.GeofenceRequest.Add(_mapper.ToCircularGeofenceRequest(item));
                }
                
                geofenceservice.CircularGeofenceResponse geofenceResponse = await _GeofenceServiceClient.CreateCircularGeofenceAsync(geofenceRequest);
                ///var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Failed)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Conflict)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Create Circular  method in Geofence controller", request[0].Id, request[0].Id, JsonConvert.SerializeObject(request),
                   Request);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Geofence controller", request[0].Id, request[0].Id, JsonConvert.SerializeObject(request),
                  Request);
                _logger.Error(null, ex);
                // check for fk violation
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
        [Route("deletegeofence")]
        public async Task<IActionResult> DeleteGeofence(DeleteGeofences request)
        {
            GeofenceDeleteResponse objGeofenceDeleteResponse = new GeofenceDeleteResponse();
            DeleteRequest objDeleteRequest = new DeleteRequest();
            Alert.LandmarkIdRequest landmarkIdRequest = new Alert.LandmarkIdRequest();
            try
            {
                foreach (var item in request.GeofenceIds)
                {
                    landmarkIdRequest.LandmarkId.Add(item);
                }
                Alert.LandmarkIdExistResponse isLandmarkavalible = await _alertServiceClient.IsLandmarkActiveInAlertAsync(landmarkIdRequest);

                if (isLandmarkavalible.IsLandmarkActive)
                {
                    return StatusCode(400, "Geofence is used in alert.");
                }
                else
                {

                    foreach (var item in request.GeofenceIds)
                    {
                        objDeleteRequest.GeofenceId.Add(item);
                    }
                    objDeleteRequest.ModifiedBy = request.ModifiedBy;
                    objGeofenceDeleteResponse = await _GeofenceServiceClient.DeleteGeofenceAsync(objDeleteRequest);

                    if (objGeofenceDeleteResponse.Code == Responsecode.Success)
                    {
                        await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                         "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "DeleteGeofence  method in Geofence controller", 0, 0, JsonConvert.SerializeObject(request),
                          Request);
                        return Ok(objGeofenceDeleteResponse);
                    }
                    else
                    {
                        return StatusCode(400, "Bad Request");
                    }
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Delete  method in Geofence controller", 0, 0, JsonConvert.SerializeObject(request),
                  Request);

                //// check for fk violation
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

        [HttpGet]
        [Route("getgeofencebygeofenceid")]
        public async Task<IActionResult> GetGeofenceByGeofenceID([FromQuery] net.atos.daf.ct2.portalservice.Entity.Geofence.GeofencebyIDEntity request)
        {
            GetGeofenceResponse response = new GetGeofenceResponse();
            IdRequest idRequest = new IdRequest();
            try
            {
                request.OrganizationId = GetContextOrgId();
                if (request.GeofenceId < 1)
                {
                    return StatusCode(400, "Bad request");
                }
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "GetGeofenceByGeofenceID  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 Request);
                if (request.OrganizationId>0)
                {
                    idRequest.OrganizationId = request.OrganizationId;
                }                
                idRequest.GeofenceId = request.GeofenceId;

                var result = await _GeofenceServiceClient.GetGeofenceByGeofenceIDAsync(idRequest);
                if (result.Id>0)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(404,"No record found ");
                }                          
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetGeofenceByGeofenceID  method in Geofence controller", Convert.ToInt32(request.GeofenceId), Convert.ToInt32(request.OrganizationId), JsonConvert.SerializeObject(request),
                 Request);

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

        [HttpGet]
        [Route("getallgeofence")]
        public async Task<IActionResult> GetAllGeofence([FromQuery] net.atos.daf.ct2.portalservice.Entity.Geofence.GeofenceEntity request)
        {
            GeofenceEntityResponceList response = new GeofenceEntityResponceList();            
            try
            {
                request.OrganizationId = GetContextOrgId();
                GeofenceEntityRequest objGeofenceRequest = new GeofenceEntityRequest();
                    objGeofenceRequest.OrganizationId = request.OrganizationId;
                    objGeofenceRequest.CategoryId = request.CategoryId;
                    objGeofenceRequest.SubCategoryId = request.SubCategoryId;

                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                    "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "GetAllGeofence  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                     Request);

                    var result = await _GeofenceServiceClient.GetAllGeofenceAsync(objGeofenceRequest);
                    return Ok(result);               
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetAllGeofence  method in Geofence controller", Convert.ToInt32(request.OrganizationId), Convert.ToInt32(request.CategoryId), JsonConvert.SerializeObject(request),
                 Request);
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
        [Route("updatepolygongeofence")]
        public async Task<IActionResult> UpdatePolygonGeofence(GeofenceUpdateEntity request)
        {
            try
            {
                
                _logger.Info("UpdatePolygonGeofence method in geofence API called.");

                // Validate Admin Privilege
                if (request.OrganizationId == 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global geofence.");
                }
                else
                {
                    request.OrganizationId = GetContextOrgId();
                }
                // Validation 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The Geofence name is required.");
                }
                var geofenceRequest = new geofenceservice.GeofencePolygonUpdateRequest();
                geofenceRequest = _mapper.ToGeofenceUpdateRequest(request);
                geofenceservice.GeofencePolygonUpdateResponce geofenceResponse = await _GeofenceServiceClient.UpdatePolygonGeofenceAsync(geofenceRequest);
                ///var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Failed)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Conflict)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Update polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   Request);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, "Geofence Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  Request);
                _logger.Error(null, ex);
                // check for fk violation
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

        [HttpPost]
        [Route("bulkimportgeofence")]
        public async Task<IActionResult> BulkImportGeofence(List<Geofence> requests)
        {
            try
            {
                foreach(var item in requests)
                {
                     item.OrganizationId = GetContextOrgId();
                }
                
                if (requests?.Count() == 0)
                {
                    return StatusCode(400, "Bulk import geofence payload is having no items.");
                }
                var bulkGeofenceRequest = new geofenceservice.BulkGeofenceRequest();
                foreach (var request in requests)
                    bulkGeofenceRequest.GeofenceRequest.Add(_mapper.ToGeofenceRequest(request));
                var response = await _GeofenceServiceClient.BulkImportGeofenceAsync(bulkGeofenceRequest);
                return StatusCode((int)response.Code, response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.BULK, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"BulkImportGeofence method Failed", 1, 2, JsonConvert.SerializeObject(requests),
                  Request);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, "Unknown: There is error while processing the request. please try again later. If issue persist, then contact DAF support team.");
            }
        }

        [HttpPut]
        [Route("updatecirculargeofence")]
        public async Task<IActionResult> UpdateCircularGeofence(GeofenceUpdateEntity request)
        {
            try
            {
                 _logger.Info("UpdateCircularGeofence method in Geofence API called.");

                // Validate Admin Privilege
                if (request.OrganizationId == 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global geofence.");
                }
                else
                {
                    request.OrganizationId = GetContextOrgId();
                }

                // Validation 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The Geofence name is required.");
                }
                var geofenceRequest = new geofenceservice.GeofenceCircularUpdateRequest();
                geofenceRequest = _mapper.ToCircularGeofenceUpdateRequest(request);
                geofenceservice.GeofenceCircularUpdateResponce geofenceResponse = await _GeofenceServiceClient.UpdateCircularGeofenceAsync(geofenceRequest);
                ///var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Failed)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Conflict)
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Update Circular method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   Request);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, "Geofence Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update Circular method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  Request);
                _logger.Error(null, ex);
                // check for fk violation
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
        [HttpGet]
        [Route("getallgeofences")]
        public async Task<IActionResult> GetAllGeofences([FromQuery] GeofenceFilter request)
        {
            GeofenceListResponse response = new GeofenceListResponse();
            try
            {
                request.OrganizationId = GetContextOrgId();
                GeofenceRequest geofenceRequest = new GeofenceRequest();
                geofenceRequest.Id = request.Id;
                geofenceRequest.OrganizationId = request.OrganizationId;
                geofenceRequest.CategoryId = request.CategoryId;
                geofenceRequest.SubCategoryId = request.SubCategoryId;
                
                var result = await _GeofenceServiceClient.GetAllGeofencesAsync(geofenceRequest);
                
                if (result != null && result.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "GetAllGeofences  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 Request);

                    return Ok(result.Geofences);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error.");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetAllGeofences  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 Request);
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }

        }
        #endregion
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
    }
}
