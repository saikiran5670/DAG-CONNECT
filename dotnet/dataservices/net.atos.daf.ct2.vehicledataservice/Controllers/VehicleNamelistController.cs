using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using net.atos.daf.ct2.vehicledataservice.Entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    //[Authorize(Policy = AccessPolicies.MAIN_NAMELIST_ACCESS_POLICY)]
    public class VehicleNamelistController : ControllerBase
    {
        private readonly ILogger<VehicleMileageController> _logger;
        private readonly IVehicleManager _vehicleManager;
        private readonly IAccountManager _accountManager;
        private readonly IAuditTraillib _auditTrail;
        private readonly IAccountIdentityManager _accountIdentityManager;
        public VehicleNamelistController(IVehicleManager vehicleManager, IAccountManager accountManager
            , ILogger<VehicleMileageController> logger, IAuditTraillib auditTrail, IAccountIdentityManager accountIdentityManager)
        {
            this._vehicleManager = vehicleManager;
            this._accountManager = accountManager;
            _auditTrail = auditTrail;
            this._logger = logger;
            this._accountIdentityManager = accountIdentityManager;
        }

        [HttpGet]
        [Route("namelist")]
        public async Task<IActionResult> GetVehicleNamelist([FromQuery] VehicleNamelistRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Vehicle Namelist Service", nameof(GetVehicleNamelist), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get namelist method vehicle namelist service", 1, 2, JsonConvert.SerializeObject(request), 0, 0);

                var result = ValidateRequest(request);
                if (result is OkObjectResult)
                {
                    var isInMillis = (bool)(result as ObjectResult).Value;
                    long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                    int accountId = 0, orgId = 0;
                    VehicleNamelistSSOContext context = VehicleNamelistSSOContext.None;
                    if (!string.IsNullOrEmpty(request.Token) || !string.IsNullOrEmpty(request.Org))
                    {
                        string tokenVal;
                        if (!string.IsNullOrEmpty(request.Token))
                        {
                            context = VehicleNamelistSSOContext.Token;
                            tokenVal = request.Token.Trim();

                            var ssoResult = await _accountIdentityManager.ValidateSSOTokenForNamelist(tokenVal);
                            if (ssoResult.StatusCode == HttpStatusCode.NotFound)
                                return GenerateErrorResponse(ssoResult.StatusCode, nameof(request.Token), ssoResult.Message);

                            accountId = ssoResult.AccountID;
                            orgId = ssoResult.OrganizationID;
                        }
                        else
                        {
                            context = VehicleNamelistSSOContext.Org;
                            tokenVal = request.Org.Trim();

                            var ssoResult = await _accountIdentityManager.ValidateSSOTokenForNamelist(tokenVal);
                            if (ssoResult.StatusCode == HttpStatusCode.NotFound)
                                return GenerateErrorResponse(ssoResult.StatusCode, nameof(request.Token), ssoResult.Message);

                            orgId = ssoResult.OrganizationID;
                        }
                    }
                    else
                    {
                        var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                        var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());
                        var orgs = await _accountManager.GetAccountOrg(account.Id);

                        accountId = account.Id;
                        orgId = orgs.First().Id;
                    }

                    var response = await _vehicleManager.GetVehicleNamelist(request.Since, isInMillis, accountId, orgId, context);

                    return new JsonResult(new
                    {
                        RequestTimestamp = currentdatetime,
                        Vehicles = response.VehicleRelations
                    }, new JsonSerializerOptions() { IgnoreNullValues = true });

                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Vehicle Namelist data.");
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Vehicle Namelist Service", nameof(GetVehicleNamelist), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get namelist method vehicle namelist service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        private bool ValidateSinceParameter(ref string since, out bool isNumeric)
        {
            isNumeric = long.TryParse(since, out _);
            if (isNumeric)
            {
                string sTimezone = "UTC";
                try
                {
                    string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, null);
                    if (!DateTime.TryParse(converteddatetime, out DateTime dDate))
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

        private IActionResult ValidateRequest(VehicleNamelistRequest request)
        {
            bool isInMillis = false;
            if (!string.IsNullOrEmpty(request.Since))
            {
                var since = request.Since;
                if (!ValidateSinceParameter(ref since, out isInMillis))
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(request.Since));
                }
                request.Since = since;
            }

            if (!string.IsNullOrEmpty(request.Token) && !string.IsNullOrEmpty(request.Org))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, $"{ nameof(request.Token) }/{ nameof(request.Org) }");

            return new OkObjectResult(isInMillis);
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value, string message = null)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = string.IsNullOrEmpty(message) ? "INVALID_PARAMETER" : message,
                Value = value
            });
        }
    }
}
