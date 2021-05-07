using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicledataservice.Entity;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using net.atos.daf.ct2.audit;
using System.Linq;
using net.atos.daf.ct2.vehicle.response;
using net.atos.daf.ct2.account;
using System.Security.Claims;

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
        private readonly IAuditTraillib AuditTrail;
        public VehicleNamelistController(IVehicleManager _vehicleManager, IAccountManager _accountManager, ILogger<VehicleNamelistController> _logger, IAuditTraillib _AuditTrail)
        {
            vehicleManager = _vehicleManager;
            accountManager = _accountManager;
            AuditTrail = _AuditTrail;
            logger = _logger;
        }
        [HttpGet]
        [Route("namelist")]
        public async Task<IActionResult> GetVehicleNamelist(string since)
        {
            try
            {
                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

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
                logger.LogError(ex.Message);
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

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string parameter)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = "INVALID_PARAMETER",
                Value = parameter + " parameter has an invalid value."
            });
        }
    }
}
