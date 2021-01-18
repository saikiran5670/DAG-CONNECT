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
        public async Task<IActionResult> Auth([FromBody] IdentityEntity.Identity user)
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
                    AccountEntity.AccountIdentity response = await accountIdentityManager.Login(user);
                    return Ok(response);
                    // if(response!=null && response.Result==null)
                    //     return Ok(result);
                    // else 
                    //     return StatusCode((int) response.StatusCode,response.Result);

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
    }
}
