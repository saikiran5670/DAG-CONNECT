using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.portalservice.Common;
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
        public LandmarkCorridorController(CorridorService.CorridorServiceClient corridorServiceClient, AuditHelper auditHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorServiceClient = corridorServiceClient;
            _auditHelper = auditHelper;
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
    }

}
