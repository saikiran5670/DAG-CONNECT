using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AccountBusinessService = net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.portalservice.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Newtonsoft.Json;
using log4net;
using net.atos.daf.ct2.portalservice.Common;
using System.Reflection;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AuthenticationController : ControllerBase
    {
        //private readonly ILogger<AuthenticationController> _logger;
        private readonly AuditHelper _auditHelper;

        private ILog _logger;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        public AuthenticationController(AccountBusinessService.AccountService.AccountServiceClient accountClient, AuditHelper auditHelper)
        {
            _accountClient = accountClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditHelper = auditHelper;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            AccountBusinessService.IdentityRequest identityRequest = new AccountBusinessService.IdentityRequest();
            try
            {
                if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))
                {
                    var authHeader = Request.Headers["Authorization"].ToString().Replace("Basic ", "");
                    var identity = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                    var arrUsernamePassword = identity.Split(':');
                    if (string.IsNullOrEmpty(arrUsernamePassword[0]))
                    {
                        return StatusCode(401, "invalid_grant: The username is Empty.");
                    }
                    else if (string.IsNullOrEmpty(arrUsernamePassword[1]))
                    {
                        return StatusCode(401, "invalid_grant: The password is Empty.");
                    }
                    else
                    {

                        identityRequest.UserName = arrUsernamePassword[0];
                        identityRequest.Password = arrUsernamePassword[1];
                        AccountBusinessService.AccountIdentityResponse response = new AccountBusinessService.AccountIdentityResponse();

                        response = await _accountClient.AuthAsync(identityRequest).ResponseAsync;
                        if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                        {
                            Identity.Identity accIdentity = new Identity.Identity();
                            accIdentity.AccountInfo = new Identity.Account(); ;
                            accIdentity.AccountInfo.Id = response.AccountInfo.Id;
                            accIdentity.AccountInfo.EmailId = response.AccountInfo.EmailId;
                            accIdentity.AccountInfo.Salutation = response.AccountInfo.Salutation;
                            accIdentity.AccountInfo.FirstName = response.AccountInfo.FirstName;
                            accIdentity.AccountInfo.LastName = response.AccountInfo.LastName;
                            accIdentity.AccountInfo.Organization_Id = response.AccountInfo.OrganizationId;
                            accIdentity.AccountInfo.PreferenceId = response.AccountInfo.PreferenceId;
                            accIdentity.AccountInfo.BlobId = response.AccountInfo.BlobId;
                            if (response.AccOrganization != null && response.AccOrganization.Count > 0)
                            {
                                accIdentity.AccountOrganization = new List<Identity.KeyValue>();
                                Identity.KeyValue keyValue = new Identity.KeyValue();
                                foreach (var accOrg in response.AccOrganization)
                                {
                                    keyValue = new Identity.KeyValue();
                                    keyValue.Id = accOrg.Id;
                                    keyValue.Name = accOrg.Name;
                                    accIdentity.AccountOrganization.Add(keyValue);
                                }
                            }
                            if (response.AccountRole != null && response.AccountRole.Count > 0)
                            {
                                accIdentity.AccountRole = new List<AccountOrgRole>();
                                Identity.AccountOrgRole accRole = new Identity.AccountOrgRole();
                                foreach (var accrole in response.AccountRole)
                                {
                                    accRole = new Identity.AccountOrgRole();
                                    accRole.Id = accrole.Id;
                                    accRole.Name = accrole.Name;
                                    accRole.Organization_Id = accrole.OrganizationId;
                                    accIdentity.AccountRole.Add(accRole);
                                }
                            }
                            //if (!string.IsNullOrEmpty(response.TokenIdentifier))
                            // {
                            try
                            {
                                HttpContext.Session.SetString("session_id", response.TokenIdentifier);
                                _logger.Info($"Value set in Session - { response.TokenIdentifier }");
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Error while setting value in session", ex);
                            }
                            
                            var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Email, accIdentity.AccountInfo.EmailId),
                                    new Claim(ClaimTypes.Name, accIdentity.AccountInfo.FirstName)
                                };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                            // }


                            await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Authentication Component",
                                    "Authentication service", Entity.Audit.AuditTrailEnum.Event_type.LOGIN, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                    "RemoveRoles  method in Authentication controller", 0, response.AccountInfo.Id, JsonConvert.SerializeObject(identityRequest),
                                     Request);


                            return Ok(accIdentity);
                        }//To Do: Need to fix once we stream line the responceCode class in gRPC Account service.
                        else if (response != null && (response.Code == AccountBusinessService.Responcecode.FoundRedirect))
                        {
                            return StatusCode((int)response.Code, response.ResetPasswordExpiryResponse);
                        }
                        else if (response != null && (response.Code == AccountBusinessService.Responcecode.Unauthorized))
                        {
                            return StatusCode((int)response.Code, response.Message);
                        }
                        else if (response != null && response.Code == AccountBusinessService.Responcecode.Forbidden)
                        {
                            return StatusCode(403, response.Message);
                        }
                        else if (response != null && response.Code == AccountBusinessService.Responcecode.NotFound)
                        {
                            return StatusCode(404, response.Message);
                        }
                        else if (response != null && response.Code == AccountBusinessService.Responcecode.Failed)
                        {
                            return StatusCode(500, response.Message);
                        }
                        else
                        {
                            return StatusCode(500, "Unknown :- Please contact system administrator.");
                        }
                    }
                }
                else
                {
                    return StatusCode(401, "The authorization header is either empty or isn't Basic.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Authentication Component",
            "Authentication service", Entity.Audit.AuditTrailEnum.Event_type.LOGIN, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
            "RemoveRoles  method in Authentication controller", 0, 0, JsonConvert.SerializeObject(identityRequest),
             Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Please contact system administrator. " + ex.Message);
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            AccountBusinessService.LogoutRequest request = new AccountBusinessService.LogoutRequest();
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                string sessionid = HttpContext.Session.GetString("session_id");
                if (!string.IsNullOrEmpty(sessionid))
                {

                    request.TokenId = sessionid;
                    await _accountClient.LogoutAsync(request);
                }
                return Ok(new { Message = "You are logged out" });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Please contact system administrator. " + ex.Message);
            }
        }
    }
}
