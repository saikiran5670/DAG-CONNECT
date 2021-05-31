using System;
using System.Threading.Tasks;
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
        private readonly ILogger logger;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        private readonly AccountComponent.IAccountManager accountManager;
        public AuthenticationController(AccountComponent.IAccountIdentityManager _accountIdentityManager, AccountComponent.IAccountManager _accountManager, ILogger<AuthenticationController> _logger)
        {
            accountIdentityManager = _accountIdentityManager;
            accountManager = _accountManager;
            logger = _logger;
        }
        [HttpPost]
        [Route("auth")]
        //In case, to generate only account token 
        public async Task<IActionResult> GenerateToken()
        {
            string identity = string.Empty;
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

                        IdentityEntity.AccountToken response = await accountIdentityManager.GenerateTokenGUID(user);
                        if (response != null && response.statusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.AccessToken))
                        {
                            //Check for feature access
                            var isExists = await accountManager.CheckForFeatureAccessByEmailId(user.UserName, Constants.MainPolicy);
                            if (!isExists)
                                return StatusCode(403, string.Empty);

                            AuthToken authToken = new AuthToken();
                            authToken.access_token = response.AccessToken;
                            authToken.expires_in = response.ExpiresIn;
                            authToken.token_type = response.TokenType;
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
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }

        //[HttpPost]        
        //[Route("validate")]
        //public async Task<IActionResult> Validate([FromBody] string token)
        //{
        //    bool valid=false;
        //    try 
        //    {
        //        if(string.IsNullOrEmpty(token))
        //        {
        //            return StatusCode(401);
        //        }
        //        else
        //        {
        //            AccountEntity.ValidTokenResponse response = await accountIdentityManager.ValidatTokeneGuid(token);
        //            valid = (response!=null) && (response.Valid==true) ? true : false;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        valid = false;
        //        logger.LogError(ex.Message +" " +ex.StackTrace);
        //        return StatusCode(500);
        //    }           
        //    return Ok(valid); 
        //}

        [HttpPost]
        [Route("signout")]
        public async Task<IActionResult> signout([FromBody] string token)
        {
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return StatusCode(401);
                }
                else
                {
                    valid = await accountIdentityManager.LogoutByJwtToken(token);
                    if (valid)
                    {
                        return StatusCode(200);
                    }
                    else
                    {
                        return StatusCode(401);
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }

    }
}