using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AccountComponent = net.atos.daf.ct2.account;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using AccountPreferenceComponent = net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.authenticationservicerest.Entity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;

namespace net.atos.daf.ct2.authenticationservicerest.Controllers
{
    [ApiController]
    // [Route("[controller]")]
    public class AuthenticationController: ControllerBase
    {
        private readonly ILogger logger;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        public AuthenticationController(AccountComponent.IAccountIdentityManager _accountIdentityManager,ILogger<AuthenticationController> _logger)
        {
            accountIdentityManager = _accountIdentityManager;
            logger =_logger;
        }
        [HttpPost]
        [Route("auth")]
        //In case, to generate only account token 
        public async Task<IActionResult> GenerateToken()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))
                {
                    var authHeader = Request.Headers["Authorization"].ToString().Replace("Basic ", "");
                    var identity = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                    var arrUsernamePassword = identity.Split(':');
                    if (string.IsNullOrEmpty(arrUsernamePassword[0].Trim()))
                    {
                        return StatusCode(401,string.Empty);
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
                        IdentityEntity.AccountToken response = await accountIdentityManager.GenerateToken(user);
                        if (response != null && response.statusCode==System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.AccessToken))
                        {
                            AuthToken authToken = new AuthToken();
                            authToken.AccessToken = response.AccessToken;
                            authToken.ExpiresIn = response.ExpiresIn;
                            authToken.TokenType = response.TokenType;
                            authToken.SessionState = response.SessionState;
                            authToken.Scope = response.Scope;
                            return Ok(authToken);
                        }
                        else
                        {
                            return StatusCode(401,string.Empty);
                        }
                    }
                }
                else
                {
                    return StatusCode(401,string.Empty);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500,string.Empty);
            }
        }

        [HttpPost]        
        [Route("validate")]
        public async Task<IActionResult> Validate([FromBody] string token)
        {
            bool valid=false;
            try 
            {
                if(string.IsNullOrEmpty(token))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else
                {
                    valid = await accountIdentityManager.ValidateToken(token);
                }
            }
            catch(Exception ex)
            {
                valid = false;
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500);
            }           
            return Ok(valid); 
        }
    }
}