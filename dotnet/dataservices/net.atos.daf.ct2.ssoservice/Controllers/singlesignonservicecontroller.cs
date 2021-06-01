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
    [Authorize(Policy = AccessPolicies.MainAccessPolicy)]
    public class singlesignonservicecontroller : ControllerBase
    {
        private readonly ILogger<singlesignonservicecontroller> logger;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        public IConfiguration Configuration { get; }
        public singlesignonservicecontroller(
            AccountComponent.IAccountIdentityManager _accountIdentityManager,
            ILogger<singlesignonservicecontroller> _logger,
            IConfiguration configuration
            )
        {

            accountIdentityManager = _accountIdentityManager;
            Configuration = configuration;
            logger = _logger;
        }

        [HttpGet]
        [Route("sso")]
        public async Task<IActionResult> validateSSOToken(string token)
        {
            try
            {
                UserDetails _details = new UserDetails();
                if (!string.IsNullOrEmpty(token))
                {
                    SSOResponse result = await accountIdentityManager.ValidateSSOToken(token);
                    if (result != null)
                    {
                        if (result.Details != null)
                        {
                            _details.AccountID = result.Details.AccountID;
                            _details.AccountName = result.Details.AccountName;
                            _details.RoleID = result.Details.RoleID;
                            _details.OrganizationID = result.Details.OrganizationID;
                            _details.OraganizationName = result.Details.OrganizationName;
                            _details.DateFormat = result.Details.DateFormat;
                            _details.TimeZone = result.Details.TimeZone;
                            _details.UnitDisplay = result.Details.UnitDisplay;
                            _details.VehicleDisplay = result.Details.VehicleDisplay;

                            return Ok(_details);
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
                logger.LogError(ex.Message);
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
