using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.singlesignonservice.CustomAttributes;
using net.atos.daf.ct2.singlesignonservice.Entity;
using System;
using System.Threading.Tasks;
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
                    SSOTokenResponse result = await accountIdentityManager.ValidateSSOToken(token);
                    if (result != null)
                    {
                        _details.AccountID = result.AccountID;
                        _details.AccountName = result.AccountName;
                        _details.RoleID = result.RoleID;
                        _details.OrganizationID = result.OrganizationID;
                        _details.OraganizationName = result.OrganizationName;
                        _details.DateFormat = result.DateFormat;
                        _details.TimeZone = result.TimeZone;
                        _details.UnitDisplay = result.UnitDisplay;
                        _details.VehicleDisplay = result.VehicleDisplay;

                        return Ok(_details);
                    }
                    else
                    {
                        return StatusCode(404, string.Empty);
                    }
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(404, string.Empty);
            }
        }
    }
}
