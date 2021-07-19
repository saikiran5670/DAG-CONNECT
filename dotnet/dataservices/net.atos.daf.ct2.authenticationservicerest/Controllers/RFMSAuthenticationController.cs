using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.authenticationservicerest.Entity;
using AccountComponent = net.atos.daf.ct2.account;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.authenticationservicerest.CustomAttributes;

namespace net.atos.daf.ct2.authenticationservicerest.Controllers
{
    [ApiController]
    [Route("rfms3")]
    public class RFMSAuthenticationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;
        private readonly AccountComponent.IAccountManager _accountManager;

        public RFMSAuthenticationController(AccountComponent.IAccountIdentityManager accountIdentityManager, AccountComponent.IAccountManager accountManager, ILogger<AuthenticationController> logger)
        {
            this._accountIdentityManager = accountIdentityManager;
            this._accountManager = accountManager;
            this._logger = logger;
        }

        [HttpPost]
        [Route("token")]
        //In case, to generate only account token 
        public async Task<IActionResult> GenerateToken()
        {
            string identity;
            try
            {
                if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))
                {
                    var authHeader = Request.Headers["Authorization"].ToString().Replace("Basic ", "");
                    try
                    {
                        identity = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                    }
                    catch (Exception)
                    {
                        return StatusCode(400, string.Empty);
                    }
                    var arrUsernamePassword = identity.Split(':');
                    if (string.IsNullOrEmpty(arrUsernamePassword[0].Trim()))
                    {
                        return StatusCode(401, string.Empty);
                    }
                    else if (string.IsNullOrEmpty(arrUsernamePassword[1]))
                    {
                        return StatusCode(401, string.Empty);
                    }
                    else
                    {
                        IdentityEntity.Identity user = new IdentityEntity.Identity();
                        user.UserName = arrUsernamePassword[0].Trim();
                        user.Password = arrUsernamePassword[1];

                        IdentityEntity.AccountToken response = await _accountIdentityManager.GenerateToken(user);
                        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.AccessToken))
                        {
                            //Check for feature access
                            var isExists = await _accountManager.CheckForFeatureAccessByEmailId(user.UserName, Constants.MainPolicy);
                            if (!isExists)
                                return StatusCode(403, string.Empty);

                            AuthToken authToken = new AuthToken();
                            authToken.Access_token = response.AccessToken;
                            authToken.Expires_in = response.ExpiresIn;
                            authToken.Token_type = response.TokenType;
                            return Ok(authToken);
                        }
                        else
                        {
                            return StatusCode(401, string.Empty);
                        }
                    }
                }
                else
                {
                    return StatusCode(401, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }
    }
}
