using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Geofence;
using Newtonsoft.Json;
using Alert = net.atos.daf.ct2.alertservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("geofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkGeofenceController : BaseController
    {
        private readonly ILog _logger;
        private readonly GeofenceService.GeofenceServiceClient _geofenceServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Mapper _mapper;
        private readonly string _fk_Constraint = "violates foreign key constraint";
        private readonly string _socketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly Alert.AlertService.AlertServiceClient _alertServiceClient;
        public LandmarkGeofenceController(GeofenceService.GeofenceServiceClient GeofenceServiceClient, AuditHelper auditHelper
            , IHttpContextAccessor httpContextAccessor, Alert.AlertService.AlertServiceClient alertServiceClient, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _geofenceServiceClient = GeofenceServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Mapper();
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
                    bool hasRights = HasAdminPrivilege();
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
                geofenceservice.GeofenceResponse geofenceResponse = await _geofenceServiceClient.CreatePolygonGeofenceAsync(geofenceRequest);
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
                    await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Create polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   _userDetails);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(CreatePolygonGeofence)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
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
                        bool hasRights = HasAdminPrivilege();
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

                geofenceservice.CircularGeofenceResponse geofenceResponse = await _geofenceServiceClient.CreateCircularGeofenceAsync(geofenceRequest);
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
                    await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Create Circular  method in Geofence controller", request[0].Id, request[0].Id, JsonConvert.SerializeObject(request),
                   _userDetails);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, geofenceResponse.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Geofence controller", request[0].Id, request[0].Id, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(CreateCircularGeofence)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPut]
        [Route("deletegeofence")]
        public async Task<IActionResult> DeleteGeofence(DeleteGeofences request)
        {
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
                    GeofenceDeleteResponse objGeofenceDeleteResponse = await _geofenceServiceClient.DeleteGeofenceAsync(objDeleteRequest);

                    if (objGeofenceDeleteResponse.Code == Responsecode.Success)
                    {
                        await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                         "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "DeleteGeofence  method in Geofence controller", 0, 0, JsonConvert.SerializeObject(request),
                          _userDetails);
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
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Delete  method in Geofence controller", 0, 0, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(DeleteGeofence)}: With Error:-", ex);
                //// check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
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
                if (request.OrganizationId != 0)
                {
                    request.OrganizationId = GetContextOrgId();
                }
                if (request.GeofenceId < 1)
                {
                    return StatusCode(400, "Bad request");
                }
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "GetGeofenceByGeofenceID  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 _userDetails);
                if (request.OrganizationId > 0)
                {
                    idRequest.OrganizationId = request.OrganizationId;
                }
                idRequest.GeofenceId = request.GeofenceId;

                var result = await _geofenceServiceClient.GetGeofenceByGeofenceIDAsync(idRequest);
                if (result.Id > 0)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(404, "No record found ");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetGeofenceByGeofenceID  method in Geofence controller", Convert.ToInt32(request.GeofenceId), Convert.ToInt32(request.OrganizationId), JsonConvert.SerializeObject(request),
                 _userDetails);
                _logger.Error($"{nameof(GetGeofenceByGeofenceID)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getallgeofence")]
        public async Task<IActionResult> GetAllGeofence([FromQuery] net.atos.daf.ct2.portalservice.Entity.Geofence.GeofenceEntity request)
        {
            GeofenceEntityResponceList response = new GeofenceEntityResponceList();
            try
            {
                if (request.OrganizationId != 0)
                {
                    request.OrganizationId = GetContextOrgId();
                }
                GeofenceEntityRequest objGeofenceRequest = new GeofenceEntityRequest();
                objGeofenceRequest.OrganizationId = request.OrganizationId;
                objGeofenceRequest.CategoryId = request.CategoryId;
                objGeofenceRequest.SubCategoryId = request.SubCategoryId;

                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "GetAllGeofence  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 _userDetails);

                var result = await _geofenceServiceClient.GetAllGeofenceAsync(objGeofenceRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetAllGeofence  method in Geofence controller", Convert.ToInt32(request.OrganizationId), Convert.ToInt32(request.CategoryId), JsonConvert.SerializeObject(request),
                 _userDetails);
                _logger.Error($"{nameof(GetAllGeofence)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
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
                    bool hasRights = HasAdminPrivilege();
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
                geofenceservice.GeofencePolygonUpdateResponce geofenceResponse = await _geofenceServiceClient.UpdatePolygonGeofenceAsync(geofenceRequest);
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
                    await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Update polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   _userDetails);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, "Geofence Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(UpdatePolygonGeofence)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("bulkimportgeofence")]
        public async Task<IActionResult> BulkImportGeofence(List<Geofence> requests)
        {
            try
            {
                foreach (var item in requests)
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
                var response = await _geofenceServiceClient.BulkImportGeofenceAsync(bulkGeofenceRequest);
                return StatusCode((int)response.Code, response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.BULK, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"BulkImportGeofence method Failed", 1, 2, JsonConvert.SerializeObject(requests),
                  _userDetails);
                _logger.Error($"{nameof(BulkImportGeofence)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
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
                    bool hasRights = HasAdminPrivilege();
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
                geofenceservice.GeofenceCircularUpdateResponce geofenceResponse = await _geofenceServiceClient.UpdateCircularGeofenceAsync(geofenceRequest);
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
                    await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Update Circular method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   _userDetails);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode((int)geofenceResponse.Code, "Geofence Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update Circular method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(UpdateCircularGeofence)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }
        [HttpGet]
        [Route("getallgeofences")]
        public async Task<IActionResult> GetAllGeofences([FromQuery] GeofenceFilter request)
        {
            GeofenceListResponse response = new GeofenceListResponse();
            try
            {
                if (request.OrganizationId != 0)
                {
                    request.OrganizationId = GetContextOrgId();
                }
                GeofenceRequest geofenceRequest = new GeofenceRequest();
                geofenceRequest.Id = request.Id;
                geofenceRequest.OrganizationId = request.OrganizationId;
                geofenceRequest.CategoryId = request.CategoryId;
                geofenceRequest.SubCategoryId = request.SubCategoryId;

                var result = await _geofenceServiceClient.GetAllGeofencesAsync(geofenceRequest);

                if (result != null && result.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "GetAllGeofences  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 _userDetails);

                    return Ok(result.Geofences);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error.");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetAllGeofences  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 _userDetails);
                _logger.Error($"{nameof(GetAllGeofences)}: With Error:-", ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }

        }
        #endregion

    }
}
