using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AccountBusinessService = net.atos.daf.ct2.accountservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        private string FK_Constraint = "violates foreign key constraint";
        public AccountController(AccountBusinessService.AccountService.AccountServiceClient accountClient,ILogger<AccountController> logger)
        {
            _accountClient=accountClient;
            _logger=logger;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(AccountBusinessService.AccountRequest request)
        {
            try
            {
                // Validation 
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName))
                || (string.IsNullOrEmpty(request.LastName)) || (request.OrganizationId <=0 ))
                {
                    return StatusCode(400, "The EmailId address, first name, last name and organization id is required.");
                }
                // Length validation
                Int32 validOrgId=0; 
                if ((request.EmailId.Length > 50 ) || (request.FirstName.Length > 30 ) 
                || (request.LastName.Length>20) || !Int32.TryParse(request.OrganizationId.ToString(), out validOrgId))
                {
                    return StatusCode(400, "The EmailId address, first name, last name and organization id should be valid.");
                }

                AccountBusinessService.AccountData accountResponse = await _accountClient.CreateAsync(request);
                if (accountResponse.Code==AccountBusinessService.Responcecode.Failed 
                    && accountResponse.Message == "The duplicate account, please provide unique email address.")
                {
                    return StatusCode(409, "Duplicate Account.");
                }
                else if (accountResponse.Code==AccountBusinessService.Responcecode.Failed
                    && accountResponse.Message == "There is an error creating account.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else
                {
                    return Ok(accountResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Create : " + ex.Message + " " + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }
                [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(AccountBusinessService.AccountRequest request)
        {
            try
            {
                // Validation 
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName)) || (string.IsNullOrEmpty(request.LastName)))
                {
                    return StatusCode(400, "The EmailId address, first name, last name is required.");
                }
                // Length validation
                Int32 validOrgId=0; 
                if ((request.EmailId.Length > 50 ) || (request.FirstName.Length > 30 ) 
                || (request.LastName.Length>20) || !Int32.TryParse(request.OrganizationId.ToString(), out validOrgId))
                {
                    return StatusCode(400, "The EmailId address, first name, last name and organization id should be valid.");
                }
                AccountBusinessService.AccountData accountResponse = await _accountClient.UpdateAsync(request);
                if (accountResponse.Code==AccountBusinessService.Responcecode.Failed 
                    && accountResponse.Message == "The duplicate account, please provide unique email address.")
                {
                    return StatusCode(409, "Duplicate Account.");
                }
                else if (accountResponse.Code==AccountBusinessService.Responcecode.Failed
                    && accountResponse.Message == "There is an error creating account.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else
                {
                    return Ok(accountResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Update : " + ex.Message + " " + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }                
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(string EmailId, int AccountId, int OrganizationId)
        {
            try
            {
                // Validation                 
                if ((string.IsNullOrEmpty(EmailId)) || (Convert.ToInt32(AccountId) <= 0) || (Convert.ToInt32(OrganizationId) <= 0))
                {
                    return StatusCode(400, "The Email address, account id and organization id is required.");
                }
                AccountBusinessService.AccountRequest accountRequest = new AccountBusinessService.AccountRequest();
                accountRequest.Id= AccountId;
                accountRequest.EmailId = EmailId;
                accountRequest.OrganizationId = OrganizationId;
                AccountBusinessService.AccountResponse response = await _accountClient.DeleteAsync(accountRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success) 
                    return Ok(accountRequest);
                else 
                    return StatusCode(404, "Account not configured.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
                [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword(AccountBusinessService.AccountRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmailId) || string.IsNullOrEmpty(request.Password))
                {
                    return StatusCode(404, "The Email address and password is required.");
                }
                AccountBusinessService.AccountResponse response = await _accountClient.ChangePasswordAsync(request);
                if (response.Code == AccountBusinessService.Responcecode.Success) 
                    return Ok("Password has been changed.");
                else 
                    return StatusCode(404, "Account not configured.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get(AccountBusinessService.AccountFilter accountFilter)
        {
            try
            {
                if (string.IsNullOrEmpty(accountFilter.Email) && string.IsNullOrEmpty(accountFilter.Name)
                    && (accountFilter.Id <= 0) && (accountFilter.OrganizationId <= 0) && (string.IsNullOrEmpty(accountFilter.AccountIds))
                    && (accountFilter.AccountGroupId <= 0)
                    )
                {
                    return StatusCode(404, "One of the parameter to filter account is required.");
                }
                
                AccountBusinessService.AccountDataList  response = await _accountClient.GetAsync(accountFilter);
                if (response!=null || (response!=null && response.Code==AccountBusinessService.Responcecode.Failed))
                {
                    return StatusCode(404, "Accounts is not found.");
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }        
            
            }
}
