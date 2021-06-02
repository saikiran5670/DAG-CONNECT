using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Corridor;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("corridor")]
    public class LandmarkCorridorController : BaseController
    {

        private ILog _logger;
        private readonly CorridorService.CorridorServiceClient _corridorServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly CorridorMapper _corridorMapper;

        public LandmarkCorridorController(CorridorService.CorridorServiceClient corridorServiceClient, AuditHelper auditHelper, AccountPrivilegeChecker privilegeChecker, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper) : base(_httpContextAccessor, sessionHelper, privilegeChecker)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorServiceClient = corridorServiceClient;
            _auditHelper = auditHelper;
            _corridorMapper = new CorridorMapper();
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }

        [HttpGet]
        [Route("getcorridorlist")]
        public async Task<IActionResult> GetCorridorList([FromQuery] net.atos.daf.ct2.portalservice.Entity.POI.CorridorRequest request)
        {
            try
            {
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "Organization Id is required.");
                }

                _logger.Info("GetCorridorList method in POI API called.");
                net.atos.daf.ct2.corridorservice.CorridorRequest objCorridorRequest = new net.atos.daf.ct2.corridorservice.CorridorRequest();
                objCorridorRequest.OrganizationId = GetContextOrgId();
                objCorridorRequest.CorridorId = request.CorridorId;//non mandatory field
                var data = await _corridorServiceClient.GetCorridorListAsync(objCorridorRequest);

                if (data != null && data.Code == net.atos.daf.ct2.corridorservice.Responsecode.Success)
                {
                    if (objCorridorRequest.OrganizationId > 0 && objCorridorRequest.CorridorId > 0)
                    {
                        if (data.CorridorEditViewList != null && data.CorridorEditViewList.Count > 0)
                        {
                            return Ok(data.CorridorEditViewList);
                        }
                        else
                        {
                            return StatusCode(404, "Corridor details are not found");
                        }
                    }
                    else
                    {
                        if (data.CorridorGridViewList != null && data.CorridorGridViewList.Count > 0)
                        {
                            return Ok(data.CorridorGridViewList);
                        }
                        else
                        {
                            return StatusCode(404, "Corridor details are not found");
                        }
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
        [Route("addroutecorridor")]

        public async Task<IActionResult> AddRouteCorridor(Entity.Corridor.CorridorRequest request)
        {
            try
            {

                if (request.OrganizationId == 0)
                {
                    return StatusCode(400, "Organization_Id Required .");
                }
                if (request.ViaAddressDetails.Count > 5)
                {
                    return StatusCode(400, "You cannot enter more than 5 via Routes.");
                }
                request.OrganizationId = GetContextOrgId();
                var MapRequest = _corridorMapper.MapCorridor(request);
                var data = await _corridorServiceClient.AddRouteCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "AddRouteCorridor method in Landmark Corridor controller", data.CorridorID, data.CorridorID, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "AddRouteCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);

                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpPost]
        [Route("addexistingtripcorridor")]

        public async Task<IActionResult> AddExistingTripCorridor(ExistingTripCorridor request)
        {
            try
            {

                if (request.OrganizationId == 0)
                {
                    //bool hasRights = await HasAdminPrivilege();
                    //if (!hasRights)
                    return StatusCode(400, "Organization_Id Required .");
                }
                if (request.ExistingTrips.Count == 0)
                {
                    return StatusCode(400, "ExistingTrips required");
                }
                request.OrganizationId = GetContextOrgId();
                var MapRequest = _corridorMapper.MapExistingTripCorridorRequest(request);
                var data = await _corridorServiceClient.AddExistingTripCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "AddExistingTripCorridor method in Landmark Corridor controller", data.CorridorID, data.CorridorID, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "AddExistingTripCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);

                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpPost]
        [Route("updateexistingtripcorridor")]

        public async Task<IActionResult> UpdateExistingTripCorridor(ExistingTripCorridor request)
        {
            try
            {

                if (request.OrganizationId == 0 && request.Id == 0)
                {
                    //bool hasRights = await HasAdminPrivilege();
                    //if (!hasRights)
                    return StatusCode(400, "Organization_Id and Id are Required .");
                }
                if (request.ExistingTrips.Count == 0)
                {
                    return StatusCode(400, "ExistingTrips required");
                }
                request.OrganizationId = GetContextOrgId();
                var MapRequest = _corridorMapper.MapExistingTripCorridorRequest(request);
                var data = await _corridorServiceClient.UpdateExistingTripCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "UpdateExistingTripCorridor method in Landmark Corridor controller", data.CorridorID, data.CorridorID, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "UpdateExistingTripCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);

                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("deletecorridor")]

        public async Task<IActionResult> DeleteCorridor([FromQuery] Entity.Corridor.DeleteCorridorIdRequest request)
        {
            try
            {
                bool hasRights = await HasAdminPrivilege();

                if (request.Id <= 0)
                {
                    return StatusCode(400, "Corridor id is required.");
                }
                var MapRequest = _corridorMapper.MapId(request);
                var data = await _corridorServiceClient.DeleteCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                         "DeleteCorridor method in Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.NotFound)
                {
                    return StatusCode(404, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "DeleteCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateRouteCorridor(Entity.Corridor.CorridorRequest request)
        {
            try
            {
                if (request.OrganizationId == 0)
                {
                    return StatusCode(400, "Organization Id is required.");
                }

                if (request.ViaAddressDetails.Count > 5)
                {
                    return StatusCode(400, "You cannot enter more than 5 via Routes.");
                }
                request.OrganizationId = GetContextOrgId();
                UpdateRouteCorridorRequest objUpdateRouteCorridorRequest = new UpdateRouteCorridorRequest();
                objUpdateRouteCorridorRequest.Request = _corridorMapper.MapCorridor(request);
                var data = await _corridorServiceClient.UpdateRouteCorridorAsync(objUpdateRouteCorridorRequest);
                if (data != null && data.Response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "UpdateRouteCorridor method in Landmark Corridor controller", data.Response.CorridorID, data.Response.CorridorID, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(data.Response);
                }
                else if (data != null && data.Response.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Response.Message);
                }
                else
                {
                    return StatusCode(500, data.Response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "UpdateRouteCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message}  {ex.StackTrace}");
            }
        }
    }

}
