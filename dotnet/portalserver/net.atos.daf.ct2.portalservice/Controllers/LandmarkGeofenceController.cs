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

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("geofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkGeofenceController : ControllerBase
    {
        private readonly GeofenceService.GeofenceServiceClient _GeofenceServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.Geofence.Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public LandmarkGeofenceController(GeofenceService.GeofenceServiceClient GeofenceServiceClient, AuditHelper auditHelper)
        {
            _GeofenceServiceClient = GeofenceServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Entity.Geofence.Mapper();
        }

        #region Geofence

        [HttpPost]
        [Route("createpolygongeofence")]
        public async Task<IActionResult> CreatePolygonGeofence(Geofence request)
        {
            try
            {
                // _logger.Info("Update method in vehicle API called.");

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
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("createcircularofence")]
        public async Task<IActionResult> CreateCircularGeofence(List<CircularGeofence> request)
        {
            try
            {
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
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("deletegeofence")]
        public async Task<IActionResult> DeleteGeofence([FromQuery] net.atos.daf.ct2.portalservice.Entity.Geofence.GeofenceDeleteEntity request)
        {
            GeofenceDeleteResponse objGeofenceDeleteResponse = new GeofenceDeleteResponse();
            DeleteRequest objDeleteRequest = new DeleteRequest();
            try
            {
                List<int> lstGeofenceId = new List<int>();
                foreach (var item in request.GeofenceId)
                {
                    lstGeofenceId.Add(item);
                }
                if (lstGeofenceId.Count > 0)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                     "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "DeleteGeofence  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                      Request);                    
                    objDeleteRequest.GeofenceId.Add(lstGeofenceId);
                    objDeleteRequest.OrganizationId = request.OrganizationId;
                    objGeofenceDeleteResponse = await _GeofenceServiceClient.DeleteGeofenceAsync(objDeleteRequest);
                    return Ok(objGeofenceDeleteResponse);
                }
                else
                {
                    return StatusCode(400, "Bad Request");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Delete  method in Geofence controller",Convert.ToInt32(request.GeofenceId),Convert.ToInt32(request.OrganizationId), JsonConvert.SerializeObject(request),
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
                if (request.OrganizationId<1)
                {
                    return StatusCode(400, "Bad request");
                }
                if (request.GeofenceId < 1)
                {
                    return StatusCode(400, "Bad request");
                }
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "GetGeofenceByGeofenceID  method in Geofence controller", request.OrganizationId, request.OrganizationId, JsonConvert.SerializeObject(request),
                 Request);
               
                idRequest.OrganizationId = request.OrganizationId;
                idRequest.GeofenceId = request.GeofenceId;

                var result = await _GeofenceServiceClient.GetGeofenceByGeofenceIDAsync(idRequest);                
                return Ok(result);              
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
                GeofenceEntityRequest objGeofenceRequest = new GeofenceEntityRequest();
                if (request.OrganizationId > 0)
                {
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
                else
                {
                    return StatusCode(400, "Bad request");
                }
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
        [HttpPost]
        [Route("updatepolygongeofence")]
        public async Task<IActionResult> UpdatePolygonGeofence(GeofenceUpdateEntity request)
        {
            try
            {
                // _logger.Info("Update method in vehicle API called.");

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
                 "Update  method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
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
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("BulkImportGeofence")]
        public async Task<IActionResult> BulkImportGeofence(List<Geofence> requests)
        {
            try
            {
                var bulkGeofenceRequest = new geofenceservice.BulkGeofenceRequest();
                foreach (var request in requests)
                    bulkGeofenceRequest.GeofenceRequest.Add(_mapper.ToGeofenceRequest(request));
                var response = await _GeofenceServiceClient.BulkImportGeofenceAsync(bulkGeofenceRequest);
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Geofence controller", 1, 2, JsonConvert.SerializeObject(requests),
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
                return StatusCode(500, "Unknown: There is error while processing the request. please try again later. If issue persist then contact DAF support team.");
            }


        }
        #endregion
    }
}
