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

                if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responcecode.Failed
                     && geofenceResponse.Message == "There is an error creating Geofence.")
                {
                    return StatusCode(500, "There is an error creating Geofence.");
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responcecode.Conflict)
                {
                    return StatusCode(409, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Create polygon method in Geofence controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                   Request);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode(404, "Geofence Response is null");
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

                if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responcecode.Failed
                     && geofenceResponse.Message == "There is an error creating Geofence.")
                {
                    return StatusCode(500, "There is an error creating Geofence.");
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responcecode.Conflict)
                {
                    return StatusCode(409, geofenceResponse.Message);
                }
                else if (geofenceResponse != null && geofenceResponse.Code == geofenceservice.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                  "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Create Circular  method in Geofence controller", request[0].Id, request[0].Id, JsonConvert.SerializeObject(request),
                   Request);

                    return Ok(geofenceResponse);
                }
                else
                {
                    return StatusCode(404, "Geofence Response is null");
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
        public async Task<IActionResult> DeleteGeofence(DeleteRequest request)
        {
            GeofenceDeleteResponse objGeofenceDeleteResponse = new GeofenceDeleteResponse();
            try
            {
                List<int> lstGeofenceId = new List<int>();
                foreach (var item in request.GeofenceId)
                {
                    lstGeofenceId.Add(item);
                }
                objGeofenceDeleteResponse= await _GeofenceServiceClient.DeleteGeofenceAsync(request);
                return Ok(objGeofenceDeleteResponse);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                 "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Delete  method in Geofence controller",Convert.ToInt32(request.GeofenceId),Convert.ToInt32(request.OrganizationId), JsonConvert.SerializeObject(request),
                  Request);
                //_logger.Error(null, ex);
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
        public async Task<IActionResult> GetGeofenceByGeofenceID(IdRequest request)
        {
            GetGeofenceResponse response = new GetGeofenceResponse();
            try
            {                
                var result = await _GeofenceServiceClient.GetGeofenceByGeofenceIDAsync(request);                
                return Ok(result);              
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetGeofenceByGeofenceID  method in Geofence controller", Convert.ToInt32(request.GeofenceId), Convert.ToInt32(request.OrganizationId), JsonConvert.SerializeObject(request),
                 Request);

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }           
        }

        [HttpGet]
        [Route("getallgeofence")]
        public async Task<IActionResult> GetAllGeofence(GeofenceEntityRequest request)
        {
            GeofenceEntityResponceList response = new GeofenceEntityResponceList();            
            try
            {              
                GeofenceEntityRequest objGeofenceRequest = new GeofenceEntityRequest();
                objGeofenceRequest.OrganizationId = request.OrganizationId;
                objGeofenceRequest.CategoryId = request.CategoryId;
                objGeofenceRequest.SubCategoryId = request.SubCategoryId;
                var result = await _GeofenceServiceClient.GetAllGeofenceAsync(objGeofenceRequest);                       
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Geofence Component",
                "Geofence service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetAllGeofence  method in Geofence controller", Convert.ToInt32(request.OrganizationId), Convert.ToInt32(request.CategoryId), JsonConvert.SerializeObject(request),
                 Request);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
          
        }
        #endregion
    }
}
