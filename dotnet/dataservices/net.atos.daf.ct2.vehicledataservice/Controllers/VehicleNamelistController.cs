using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.response;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using net.atos.daf.ct2.vehicledataservice.Entity;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(Policy = AccessPolicies.MainNamelistAccessPolicy)]
    public class VehicleNamelistController : ControllerBase
    {
        private readonly ILogger<VehicleNamelistController> logger;
        private readonly IVehicleManager vehicleManager;
        private readonly IAccountManager accountManager;
        private readonly IAuditTraillib auditTrail;
        public VehicleNamelistController(IVehicleManager _vehicleManager, IAccountManager _accountManager, ILogger<VehicleNamelistController> _logger, IAuditTraillib _auditTrail)
        {
            vehicleManager = _vehicleManager;
            accountManager = _accountManager;
            auditTrail = _auditTrail;
            logger = _logger;
        }

        [HttpGet]
        [Route("namelist")]
        public async Task<IActionResult> GetVehicleNamelist(string since)
        {
            try
            {
                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                await auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Vehicle namelist Service", "Vehicle namelist Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get namelist method vehicle namelist service", 1, 2, since, 0, 0);

                var isValid = ValidateParameter(ref since, out bool isNumeric);
                if (isValid)
                {
                    var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                    var account = await accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());

                    var orgs = await accountManager.GetAccountOrg(account.Id);

                    VehicleNamelistResponse vehiclenamelist = new VehicleNamelistResponse();
                    vehiclenamelist = await vehicleManager.GetVehicleNamelist(since, isNumeric, account.Id, orgs.First().Id);

                    vehiclenamelist.RequestTimestamp = currentdatetime;
                    return Ok(vehiclenamelist);
                }

                return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(since));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing Vehicle Namelist data.");
                return StatusCode(500, string.Empty);
            }
        }

        private bool ValidateParameter(ref string since, out bool isNumeric)
        {
            isNumeric = long.TryParse(since, out _);
            if (isNumeric)
            {
                string sTimezone = "UTC";
                DateTime dDate;
                try
                {
                    string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, null);
                    if (!DateTime.TryParse(converteddatetime, out dDate))
                        return false;
                    else
                        since = converteddatetime;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (!(string.IsNullOrEmpty(since) || since.Equals("yesterday") || since.Equals("today")))
            {
                return false;
            }
            return true;
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = "INVALID_PARAMETER",
                Value = value
            });
        }
    }
}
