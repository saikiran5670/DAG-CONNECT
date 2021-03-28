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

namespace net.atos.daf.ct2.authenticationservicerest.Controllers
{
    [ApiController]
    public class AuthenticationController: ControllerBase
    {
        private readonly ILogger logger;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        public AuthenticationController(AccountComponent.IAccountIdentityManager _accountIdentityManager,ILogger<AuthenticationController> _logger)
        {
            accountIdentityManager =_accountIdentityManager;
            logger=_logger;
        }
        
        [HttpPost]        
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            try 
            {
                if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))  
                {  
                var authHeader = Request.Headers["Authorization"].ToString().Replace("Basic ", "");  
                var identity = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                var arrUsernamePassword = identity.Split(':');  
                if(string.IsNullOrEmpty(arrUsernamePassword[0]))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else if(string.IsNullOrEmpty(arrUsernamePassword[1]))
                {
                    return StatusCode(401,"invalid_grant: The password is Empty.");
                }
                else
                {
                    IdentityEntity.Identity user = new IdentityEntity.Identity();
                    user.UserName=arrUsernamePassword[0];
                    user.Password=arrUsernamePassword[1];
                    AccountEntity.AccountIdentity response = await accountIdentityManager.Login(user);
                    if(response!=null)
                    {
                        if(string.IsNullOrEmpty(response.tokenIdentifier))
                        {
                            return StatusCode(401,"Account is not configured.");
                        }
                        return Ok(response);
                    }
                    else 
                    {
                        return StatusCode(401,"Account is not configured.");
                    }
                }
            }
            else 
            {
                return StatusCode(401,"The authorization header is either empty or isn't Basic.");
            }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
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
                    return StatusCode(401,"invalid_grant: The token is Empty.");
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
                return StatusCode(500,"Internal Server Error.");
            }           
            return Ok(valid); 
        }
        [HttpPost]        
        [Route("auth")]
        public async Task<IActionResult> Auth()
        {
            try 
            {
                if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))  
                {  
                var authHeader = Request.Headers["Authorization"].ToString().Replace("Basic ", "");  
                var identity = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                var arrUsernamePassword = identity.Split(':');  
                if(string.IsNullOrEmpty(arrUsernamePassword[0]))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else if(string.IsNullOrEmpty(arrUsernamePassword[1]))
                {
                    return StatusCode(401,"invalid_grant: The password is Empty.");
                }
                else
                {
                    IdentityEntity.Identity user = new IdentityEntity.Identity();
                    user.UserName=arrUsernamePassword[0];
                    user.Password=arrUsernamePassword[1];
                    AuthToken authToken= new AuthToken();
                    IdentityEntity.AccountToken response = await accountIdentityManager.GenerateToken(user);
                    if(response!=null)
                    {
                        authToken.access_token = response.AccessToken;
                        authToken.expires_in = response.ExpiresIn;
                        authToken.token_type = response.TokenType;
                        authToken.session_state = response.SessionState;
                        authToken.scope = response.Scope;
                        authToken.user_name = user.UserName;
                        return Ok(authToken);
                    }
                    else 
                    {
                        return StatusCode(401,"Account is not configured.");
                    }
                }
            }
            else 
            {
                return StatusCode(401,"The authorization header is either empty or isn't Basic.");
            }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
            }            
        }
        //In case, to generate only account token 
        private async Task<IActionResult> GenerateToken([FromBody] IdentityEntity.Identity user)
        {
            try 
            {
                if(string.IsNullOrEmpty(user.UserName))
                {
                    return StatusCode(401,"invalid_grant: The username is Empty.");
                }
                else if(string.IsNullOrEmpty(user.Password))
                {
                    return StatusCode(401,"invalid_grant: The password is Empty.");
                }
                else
                {
                    IdentityEntity.AccountToken response = await accountIdentityManager.GenerateToken(user);
                    if(response!=null)
                    {
                        return Ok(response);
                    }
                    else 
                    {
                        return StatusCode(401,"Account is not configured.");
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
            }            
        }
    }
}
