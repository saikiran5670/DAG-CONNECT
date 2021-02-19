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
                if (accountResponse!=null && accountResponse.Code==AccountBusinessService.Responcecode.Failed 
                    && accountResponse.Message == "The duplicate account, please provide unique email address.")
                {
                    return StatusCode(409, "Duplicate Account.");
                }
                else if (accountResponse!=null && accountResponse.Code==AccountBusinessService.Responcecode.Failed
                    && accountResponse.Message == "There is an error creating account.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if(accountResponse!=null && accountResponse.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(accountResponse);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error.");
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
                if (accountResponse!=null && accountResponse.Code==AccountBusinessService.Responcecode.Failed 
                    && accountResponse.Message == "The duplicate account, please provide unique email address.")
                {
                    return StatusCode(409, "Duplicate Account.");
                }
                else if (accountResponse !=null && accountResponse.Code==AccountBusinessService.Responcecode.Failed
                    && accountResponse.Message == "There is an error creating account.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if(accountResponse!=null && accountResponse.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(accountResponse);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error.");
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
                if (response!=null && response.Code == AccountBusinessService.Responcecode.Success) 
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
                if (response!=null && response.Code == AccountBusinessService.Responcecode.Success) 
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
                if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(404, "Accounts is not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("getaccountdetail")]
        public async Task<IActionResult> GetAccountDetail(AccountBusinessService.AccountGroupDetailsRequest request)
        {
            try
            {
                // Validation                 
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The organization is required");
                }
                AccountBusinessService.AccountDetailsResponse response=await _accountClient.GetAccountDetailAsync(request);

                if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(404, "Accounts is not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
         // Begin Account Preference
        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreateAccountPreference(AccountBusinessService.AccountPreference request)
        {
            //Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, 
            try
            {
                // Validation                 
                if ((request.RefId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.CreatePreferenceAsync(request);
                if (preference !=null && preference.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(preference);
                }
                else
                {
                    return StatusCode(500, "Internal  Server error");
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }                
                return StatusCode(500, "Internal Server Error.");
            }
        }        
        [HttpPost]
        [Route("preference/update")]
        public async Task<IActionResult> UpdateAccountPreference(AccountBusinessService.AccountPreference request)
        {        
            try
            {
                // Validation                 
                if ((request.RefId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.UpdatePreferenceAsync(request);
                if (preference !=null && preference.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(preference);
                }
                else
                {
                    return StatusCode(500, "Internal  Server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeleteAccountPreference(int accountId)
        { 
            try
            {
                AccountBusinessService.AccountPreferenceFilter request =new AccountBusinessService.AccountPreferenceFilter();
                // Validation                 
                if (accountId <= 0)
                {
                    return StatusCode(400, "The Account Id is required");
                }
                request.RefId=accountId;
                AccountBusinessService.AccountPreferenceResponse response= await _accountClient.DeletePreferenceAsync(request);
                if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "Internal  Server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpGet]
        [Route("preference/get")]
        public async Task<IActionResult> GetAccountPreference(int accountId)
        {
            try
            {
                // Validation                 
                if ((accountId <= 0))
                {
                    return StatusCode(400, "The Account Id is required");
                }
                AccountBusinessService.AccountPreferenceFilter request =new AccountBusinessService.AccountPreferenceFilter();
                request.RefId = accountId;
                AccountBusinessService.AccountPreferenceDataList response = _accountClient.GetPreference(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "Internal  Server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        // End - Account Preference
        // End - Account Preference

        // Begin - AccessRelationship
        [HttpPost]
        [Route("accessrelationship/create")]
        public async Task<IActionResult> CreateAccessRelationship(AccountBusinessService.AccessRelationship request)
        {            
            try
            {
                // Validation                 
                if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
                }
                else if ((!request.AccessRelationType.Equals("R")) && (!request.AccessRelationType.Equals("W")))
                {
                    return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
                }
                AccountBusinessService.AccessRelationshipResponse accessRelationship = await _accountClient.CreateAccessRelationshipAsync(request);
                if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(accessRelationship);
                }
                else if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Failed 
                    && accessRelationship.Message=="The AccessType should be ReadOnly / ReadWrite.(R/W).")
                {
                    return StatusCode(400, "The AccessType should be ReadOnly / ReadWrite.(R/W).");
                }
                else
                {
                    return StatusCode(500, "Internal  Server error");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("accessrelationship/update")]
        public async Task<IActionResult> UpdateAccessRelationship(AccountBusinessService.AccessRelationship request)
        {
            try
            {
                // Validation                 
                if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
                }
                else if ((!request.AccessRelationType.Equals("R")) && (!request.AccessRelationType.Equals("W")))
                {
                    return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
                }
                AccountBusinessService.AccessRelationshipResponse accessRelationship = await _accountClient.UpdateAccessRelationshipAsync(request);
                if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(accessRelationship);
                }
                else if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Failed 
                    && accessRelationship.Message=="The AccessType should be ReadOnly / ReadWrite.(R/W).")
                {
                    return StatusCode(400, "The AccessType should be ReadOnly / ReadWrite.(R/W).");
                }
                else
                {
                    return StatusCode(500, "Internal  Server error");
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
