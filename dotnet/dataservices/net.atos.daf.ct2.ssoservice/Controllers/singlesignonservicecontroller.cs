using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.singlesignonservice.CustomAttributes;
using net.atos.daf.ct2.singlesignonservice.Entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.singlesignonservice.Controllers
{
    [ApiController]
    [Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class SingleSignOnServiceController : ControllerBase
    {
        private readonly ILogger<SingleSignOnServiceController> _logger;
        private readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;
        public SingleSignOnServiceController(AccountComponent.IAccountIdentityManager accountIdentityManager, ILogger<SingleSignOnServiceController> logger)
        {
            this._accountIdentityManager = accountIdentityManager;
            this._logger = logger;
        }

        [HttpGet]
        [Route("sso")]
        public async Task<IActionResult> ValidateSSOToken(string token)
        {
            try
            {
                UserDetails details = new UserDetails();
                if (!string.IsNullOrEmpty(token))
                {
                    SSOResponse result = await _accountIdentityManager.ValidateSSOToken(token);
                    if (result != null)
                    {
                        if (result.Details != null)
                        {
                            details.AccountID = result.Details.AccountID;
                            details.AccountName = result.Details.AccountName;
                            details.RoleID = result.Details.RoleID;
                            details.OrganizationID = result.Details.OrganizationID;
                            details.OraganizationName = result.Details.OrganizationName;
                            details.DateFormat = result.Details.DateFormat;
                            details.TimeZone = result.Details.TimeZone;
                            details.UnitDisplay = result.Details.UnitDisplay;
                            details.VehicleDisplay = result.Details.VehicleDisplay;

                            return Ok(details);
                        }
                        else
                        {
                            return GenerateErrorResponse(result.StatusCode, result.Message, nameof(token));
                        }
                    }
                    else
                    {
                        return GenerateErrorResponse(result.StatusCode, result.Message, nameof(token));
                    }
                }
                else
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "MISSING_PARAMETER", nameof(token));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return GenerateErrorResponse(HttpStatusCode.NotFound, "INVALID_TOKEN", nameof(token));
            }
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode StatusCode, string Massage, string Value)
        {
            return base.StatusCode((int)StatusCode, new ErrorResponse()
            {
                ResponseCode = ((int)StatusCode).ToString(),
                Message = Massage,
                Value = Value
            });
        }
    }
}
