using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.portalservice.Account;
using AccountBusinessService = net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.portalservice.Identity;

//using net.atos.daf.ct2.authenticationservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController: ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        public AuthenticationController(AccountBusinessService.AccountService.AccountServiceClient accountClient, ILogger<AuthenticationController> logger)
        {
            _accountClient = accountClient;
            _logger = logger;
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
                    AccountBusinessService.IdentityRequest identityRequest = new AccountBusinessService.IdentityRequest();
                    identityRequest.UserName=arrUsernamePassword[0];
                    identityRequest.Password=arrUsernamePassword[1];
                    var response = await _accountClient.AuthAsync(identityRequest);
                    if(response !=null && response.Code == AccountBusinessService.Responcecode.Success)
                    {
                        Identity.Identity accIdentity = new Identity.Identity();
                        accIdentity.accountInfo = new Identity.Account(); ;
                        accIdentity.accountInfo.id = response.AccountInfo.Id;
                        accIdentity.accountInfo.emailId = response.AccountInfo.EmailId;
                        accIdentity.accountInfo.salutation = response.AccountInfo.Salutation;
                        accIdentity.accountInfo.firstName = response.AccountInfo.FirstName;
                        accIdentity.accountInfo.lastName = response.AccountInfo.LastName;
                        accIdentity.accountInfo.organization_Id = response.AccountInfo.OrganizationId;
                        accIdentity.accountInfo.preferenceId = response.AccountInfo.PreferenceId;
                        accIdentity.accountInfo.blobId = response.AccountInfo.BlobId;
                        if (response.AccOrganization != null && response.AccOrganization.Count > 0)
                        {
                            Identity.KeyValue keyValue = new Identity.KeyValue();
                            foreach (var accOrg in response.AccOrganization)
                            {
                                keyValue = new Identity.KeyValue();
                                keyValue.id = accOrg.Id;
                                keyValue.name = accOrg.Name;
                                accIdentity.accountOrganization.Add(keyValue);
                            }
                        }
                        if (response.AccountRole != null && response.AccountRole.Count > 0)
                        {
                            Identity.AccountOrgRole accRole = new Identity.AccountOrgRole();
                            foreach (var accrole in response.AccountRole)
                            {
                                accRole = new Identity.AccountOrgRole();
                                accRole.id = accrole.Id;
                                accRole.name = accrole.Name;
                                accRole.organization_Id= accrole.OrganizationId;
                                accIdentity.accountRole.Add(accRole);
                            }
                        }
                        return Ok(accIdentity); 
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
                _logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Please contact system administrator. "+ ex.Message );
            }            
        }

        //[HttpPost]        
        //[Route("validate")]
        //public async Task<IActionResult> Validate([FromBody] string token)
        //{
        //    try 
        //    {
        //        if(string.IsNullOrEmpty(token))
        //        {
        //            return StatusCode(401,"invalid_grant: The token is Empty.");
        //        }
        //        else
        //        {
        //            ValidateRequest request = new ValidateRequest();
        //            request.Token=token;
        //            ValidateResponse response = await _authClient.ValidateAsync(request);
        //            if(response !=null && response.Code == Responsecode.Success)
        //            {
        //               return Ok(response.Valid); 
        //            }
        //            else 
        //            {
        //                return StatusCode(500,"Please contact system administrator");
        //            }                    
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        logger.LogError(ex.Message +" " +ex.StackTrace);
        //        return StatusCode(500,"Please contact system administrator.");
        //    }           
        //}
    }
}
