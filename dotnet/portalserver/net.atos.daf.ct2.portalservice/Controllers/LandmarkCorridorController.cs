using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Corridor;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("corridor")]
    public class LandmarkCorridorController : ControllerBase
    {

        private ILog _logger;
        private readonly CorridorService.CorridorServiceClient _corridorServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly CorridorMapper _corridorMapper;
        private readonly HeaderObj _userDetails;
        private readonly OrganizationService.OrganizationServiceClient _organizationClient;
        public LandmarkCorridorController(CorridorService.CorridorServiceClient corridorServiceClient, AuditHelper auditHelper, Common.AccountPrivilegeChecker privilegeChecker, IHttpContextAccessor _httpContextAccessor)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorServiceClient = corridorServiceClient;
            _auditHelper = auditHelper;
            _corridorMapper = new CorridorMapper();
            _privilegeChecker = privilegeChecker;
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
                objCorridorRequest.OrganizationId = request.OrganizationId;
                objCorridorRequest.CorridorId = request.CorridorId;//non mandatory field
                var data = await _corridorServiceClient.GetCorridorListAsync(objCorridorRequest);
                if (data != null && data.Code == net.atos.daf.ct2.corridorservice.Responsecode.Success)
                {
                    if (data.CorridorList != null && data.CorridorList.Count > 0)
                    {
                        return Ok(data.CorridorList);
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
        [Route("addroutecorridor")]

        public async Task<IActionResult> AddRouteCorridor(Entity.Corridor.CorridorRequest request)
        {
            try
            {
                if (request.OrganizationId == 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global Corridor.");
                }
                if (request.ViaAddressDetails.Count >5)
                {
                    return StatusCode(400, "You cannot enter more than 5 via Routes.");
                }
                var MapRequest = _corridorMapper.MapCorridor(request);
                var data = await _corridorServiceClient.AddRouteCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
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
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "AddCategory method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);

                _logger.Error(null, ex);
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
            catch (Exception ex)
            {
                Result = false;
            }
            return Result;
        }

    }

}
