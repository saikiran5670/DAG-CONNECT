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
    [Route("api/[controller]")]
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
        [Route("Auth")]
        public async Task<IActionResult> Login([FromBody] IdentityEntity.Identity user)
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
                    AuthToken authToken= new AuthToken();
                    AccountEntity.AccountIdentity response = await accountIdentityManager.Login(user);
                    if(response!=null)
                    {
                        if(response.AccountToken!=null)
                        {
                            authToken.access_token=response.AccountToken.AccessToken;
                            authToken.expires_in=response.AccountToken.ExpiresIn;
                            authToken.token_type=response.AccountToken.TokenType;
                            authToken.session_state=response.AccountToken.SessionState;
                            authToken.scope=response.AccountToken.Scope;
                        }
                        else 
                        {
                            return StatusCode(404,"Account is not configured.");
                        }
                        if(response.AccountPreference!=null)
                        {
                            authToken.locale=response.AccountPreference.LanguageId.ToString();
                            authToken.timezone=response.AccountPreference.TimezoneId.ToString();
                            authToken.unit=response.AccountPreference.UnitId.ToString();
                            authToken.currency=response.AccountPreference.CurrencyId.ToString();
                            authToken.date_format=response.AccountPreference.DateFormatTypeId.ToString();
                        }
                        return Ok(authToken);
                    }
                    else 
                    {
                        return StatusCode(404,"Account is not configured.");
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
            }            
        }

        [HttpPost]        
        [Route("Validate")]
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
                return StatusCode(500,"Internal Server Error.");
            }           
            return Ok(valid); 
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
                        return StatusCode(404,"Account is not configured.");
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