using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.authenticationservicerest.Entity;
using AccountComponent = net.atos.daf.ct2.account;
using IdentityEntity = net.atos.daf.ct2.identity.entity;

namespace net.atos.daf.ct2.authenticationservicerest.Controllers
{
    [ApiController]
    // [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILog _logger;
        private readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;
        private readonly AccountComponent.IAccountManager _accountManager;
        public AuthenticationController(AccountComponent.IAccountIdentityManager accountIdentityManager, AccountComponent.IAccountManager accountManager)
        {
            this._accountIdentityManager = accountIdentityManager;
            this._accountManager = accountManager;
            this._logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
        [HttpPost]
        [Route("auth")]
        //In case, to generate only account token 
        public async Task<IActionResult> GenerateToken()
        {
            string identity = string.Empty;
            try
            {
                _logger.Debug($"Authentication:auth.started");
                if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))
                {
                    var authHeader = Request.Headers["Authorization"].ToString().Replace("Basic ", "");
                    try
                    {
                        identity = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Authentication:auth.", ex);
                        return StatusCode(400, string.Empty);
                    }

                    var arrUsernamePassword = identity.Split(':');
                    _logger.Debug($"Authentication:auth.name:-{arrUsernamePassword[0]}");
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

                        IdentityEntity.AccountToken response = await _accountIdentityManager.GenerateTokenGUID(user);
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
                            _logger.Debug($"Authentication:auth.name:-{arrUsernamePassword[0]} , Access_token:-{authToken.Access_token}");
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
                _logger.Error($"Authentication:auth. with Identity : {identity}.", ex);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpPost]
        [Route("signout")]
        public async Task<IActionResult> SignOut([FromBody] string token)
        {
            try
            {
                _logger.Debug($"Authentication:signout.started. with Token : {token}");
                if (string.IsNullOrEmpty(token))
                {
                    return StatusCode(401);
                }
                else
                {
                    var valid = await _accountIdentityManager.LogoutByJwtToken(token);
                    if (valid)
                    {
                        _logger.Debug($"Authentication:signout.Success. with Token : {token}");
                        return StatusCode(200);
                    }
                    else
                    {
                        _logger.Debug($"Authentication:signout.failed. with Token : {token}");
                        return StatusCode(401);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Authentication:signout.failed. with Token : {token}.", ex);
                return StatusCode(500);
            }
        }

    }
}