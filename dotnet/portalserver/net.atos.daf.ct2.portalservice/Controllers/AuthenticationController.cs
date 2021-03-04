using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.authenticationservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController: ControllerBase
    {
        private readonly ILogger logger;
        private readonly AuthService.AuthServiceClient _authClient;

        public AuthenticationController(AuthService.AuthServiceClient authClient,ILogger<AuthenticationController> _logger)
        {
            _authClient=authClient;
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
                    IdentityRequest identityRequest = new IdentityRequest();
                    identityRequest.UserName=arrUsernamePassword[0];
                    identityRequest.Password=arrUsernamePassword[1];
                    var response = await _authClient.AuthAsync(identityRequest);
                    if(response !=null && response.Code == Responsecode.Success)
                    {
                       return Ok(response); 
                    }
                    else 
                    {
                        return StatusCode(500,"Please contact system administrator 123");
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
                return StatusCode(500,"Please contact system administrator. "+ ex.Message );
            }            
        }

        [HttpPost]        
        [Route("validate")]
        public async Task<IActionResult> Validate([FromBody] string token)
        {
            try 
            {
                if(string.IsNullOrEmpty(token))
                {
                    return StatusCode(401,"invalid_grant: The token is Empty.");
                }
                else
                {
                    ValidateRequest request = new ValidateRequest();
                    request.Token=token;
                    ValidateResponse response = await _authClient.ValidateAsync(request);
                    if(response !=null && response.Code == Responsecode.Success)
                    {
                       return Ok(response.Valid); 
                    }
                    else 
                    {
                        return StatusCode(500,"Please contact system administrator");
                    }                    
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Please contact system administrator.");
            }           
        }
    }
}
