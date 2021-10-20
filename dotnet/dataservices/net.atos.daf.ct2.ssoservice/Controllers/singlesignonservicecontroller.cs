using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILog _logger;
        private readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;
        public SingleSignOnServiceController(AccountComponent.IAccountIdentityManager accountIdentityManager)
        {
            this._accountIdentityManager = accountIdentityManager;
            this._logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpGet]
        [Route("sso")]
        public async Task<IActionResult> ValidateSSOToken(string token)
        {
            _logger.Info("ValidateSSOToken dataservice api called");
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
                            _logger.Info("ValidateSSOToken dataservice api called => Token Found");
                            details.AccountID = result.Details.AccountID;
                            details.AccountName = result.Details.AccountName;
                            details.RoleID = result.Details.RoleID;
                            details.OrganizationID = result.Details.OrganizationID;
                            details.OraganizationName = result.Details.OrganizationName;
                            details.DateFormat = result.Details.DateFormat;
                            details.TimeZone = result.Details.TimeZone;
                            details.UnitDisplay = result.Details.UnitDisplay;
                            details.VehicleDisplay = result.Details.VehicleDisplay;
                            details.TimeFormat = result.Details.TimeFormat;
                            details.Language = result.Details.Language;

                            return Ok(details);
                        }
                        else
                        {
                            _logger.Info("ValidateSSOToken dataservice api called => result.Details is null");
                            return GenerateErrorResponse(result.StatusCode, result.Message, nameof(token));
                        }
                    }
                    else
                    {
                        _logger.Info("ValidateSSOToken dataservice api called => result is null");
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
                _logger.Error(null, ex);
                return GenerateErrorResponse(HttpStatusCode.NotFound, "INVALID_TOKEN", nameof(token));
            }
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string massage, string value)
        {
            return base.StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = massage,
                Value = value
            });
        }
    }
}
