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
    [Route("accountpoc")]
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
                    return Ok(accountResponse.Account);
                }
                else
                {
                    return StatusCode(500, "accountResponse is null");
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    return Ok(accountResponse.Account);
                }
                else
                {
                    return StatusCode(500, "accountResponse is null");
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    if (response.Accounts !=null && response.Accounts.Count>0)
                    {
                        return Ok(response.Accounts);
                    }
                    else
                    {
                        return StatusCode(404, "Accounts details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    if (response.AccountDetails !=null && response.AccountDetails.Count>0)
                    {
                        return Ok(response.AccountDetails);
                    }
                    else
                    {
                        return StatusCode(404, "Accounts details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    return Ok(preference.AccountPreference);
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    return Ok(preference.AccountPreference);
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeleteAccountPreference(int accountId)
        { 
            try
            {
                AccountBusinessService.IdRequest request =new AccountBusinessService.IdRequest();
                // Validation                 
                if (accountId <= 0)
                {
                    return StatusCode(400, "The Account Id is required");
                }
                request.Id=accountId;
                AccountBusinessService.AccountPreferenceResponse response= await _accountClient.DeletePreferenceAsync(request);
               if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.AccountPreference);
                }
                else
                {
                    return StatusCode(500, "preference is null" + response.Message);
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                AccountBusinessService.AccountPreferenceDataList response =await _accountClient.GetPreferenceAsync(request);
                if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    if (response.Preference !=null && response.Preference.Count>0)
                    {
                        return Ok(response.Preference);
                    }
                    else
                    {
                        return StatusCode(404, "Preference details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    return Ok(accessRelationship.AccessRelationship);
                }
                else if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Failed 
                    && accessRelationship.Message=="The AccessType should be ReadOnly / ReadWrite.(R/W).")
                {
                    return StatusCode(400, "The AccessType should be ReadOnly / ReadWrite.(R/W).");
                }
                else
                {
                    return StatusCode(500, "accessRelationship is null" + accessRelationship.Message);
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
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
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
                    return Ok(accessRelationship.AccessRelationship);
                }
                else if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Failed 
                    && accessRelationship.Message=="The AccessType should be ReadOnly / ReadWrite.(R/W).")
                {
                    return StatusCode(400, "The AccessType should be ReadOnly / ReadWrite.(R/W).");
                }
                else
                {
                    return StatusCode(500, "accessRelationship is null" + accessRelationship.Message);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("accessrelationship/delete")]
        public async Task<IActionResult> DeleteAccessRelationship(AccountBusinessService.AccessRelationshipDeleteRequest request)
        {
            try
            {
                // Validation                 
                if (request.AccountGroupId <= 0 || request.VehicleGroupId <= 0) 
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId is required");
                }
                AccountBusinessService.AccessRelationshipResponse accessRelationship  = await _accountClient.DeleteAccessRelationshipAsync(request);
                if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(accessRelationship.AccessRelationship);
                }
                else if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Failed 
                    && accessRelationship.Message=="The delete access group , Account Group Id and Vehicle Group Id is required.")
                {
                    return StatusCode(400, "The delete access group , Account Group Id and Vehicle Group Id is required.");
                }
                else
                {
                    return StatusCode(500, "accessRelationship is null" + accessRelationship.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create access relationship with exception - " + ex.Message);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("accessrelationship/get")]
        public async Task<IActionResult> GetAccessRelationship(int AccountId, int AccountGroupId,int VehicleGroupId)
        {
            try
            {
                // Validation                 
                if ((AccountId <= 0) && (AccountGroupId <= 0))
                {
                    return StatusCode(400, "The AccountId or AccountGroupId is required");
                }
                AccountBusinessService.AccessRelationshipFilter request = new AccountBusinessService.AccessRelationshipFilter();
                request.AccountId = AccountId;
                request.AccountGroupId = AccountGroupId;
                request.VehicleGroupId = VehicleGroupId;
                AccountBusinessService.AccessRelationshipDataList accessRelationship = await _accountClient.GetAccessRelationshipAsync(request);
                if (accessRelationship !=null && accessRelationship.Code==AccountBusinessService.Responcecode.Success)
                {
                    if (accessRelationship.AccessRelationship !=null && accessRelationship.AccessRelationship.Count>0)
                    {
                        return Ok(accessRelationship.AccessRelationship);
                    }
                    else
                    {
                        return StatusCode(404, "access relationship details are found.");
                    }
                }
                else if (accessRelationship != null && accessRelationship.Code==AccountBusinessService.Responcecode.Failed 
                    && accessRelationship.Message=="Please provide AccountId or AccountGroupId or VehicleGroupId to get AccessRelationship.")
                {
                    return StatusCode(400, "Please provide AccountId or AccountGroupId or VehicleGroupId to get AccessRelationship.");
                }
                else
                {
                    return StatusCode(500, accessRelationship.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accessrelatioship with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        // End - AccessRelationshhip
        // Begin - Account Group
        [HttpPost]
        [Route("accountgroup/create")]
        public async Task<IActionResult> CreateAccountGroup(AccountBusinessService.AccountGroupRequest request)
        {
            try
            {
                // Validation                 
                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(400, "The AccountGroup name is required");
                }
                AccountBusinessService.AccountGroupResponce response= await _accountClient.CreateGroupAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.AccountGroup);
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is null " + response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("accountgroup/update")]
        public async Task<IActionResult> UpdateAccountGroup(AccountBusinessService.AccountGroupRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (string.IsNullOrEmpty(request.Name)))
                {
                    return StatusCode(400, "The AccountGroup name and id is required");
                }
                
                AccountBusinessService.AccountGroupResponce response = await _accountClient.UpdateGroupAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.AccountGroup);
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is null " + response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPut]
        [Route("accountgroup/delete")]
        public async Task<IActionResult> DeleteAccountGroup(int id)
        {
            try
            {
                // Validation                 
                if ((Convert.ToInt32(id) <= 0))
                {
                    return StatusCode(400, "The account group id is required.");
                }
                AccountBusinessService.IdRequest request=new AccountBusinessService.IdRequest();
                request.Id=id;
                AccountBusinessService.AccountGroupResponce response = await _accountClient.RemoveGroupAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.AccountGroup);
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is null " + response.Message);
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group :DeleteGroup with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("accountgroup/addaccounts")]
        public async Task<IActionResult> AddAccountsToGroup(AccountBusinessService.AccountGroupRefRequest request)
        {
            try
            {
                // Validation  
                if (request == null)
                {
                    return StatusCode(400, "The AccountGroup account is required");
                }
                AccountBusinessService.AccountGroupRefResponce response =await _accountClient.AddAccountToGroupsAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "AccountGroupRefResponce is null or " + response.Message);
                }   
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group :DeleteGroup with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPut]
        [Route("accountgroup/deleteaccounts")]
        public async Task<IActionResult> DeleteAccountFromGroup(int id)
        {
            try
            {
                //Need to confrim gRPC service method name.
                return Ok("true");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group reference :DeleteAccountsGroupReference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        
        [HttpPost]
        [Route("accountgroup/get")]
        public async Task<IActionResult> GetAccountGroup(AccountBusinessService.AccountGroupFilterRequest request)
        {            
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                AccountBusinessService.AccountGroupDataList response =await _accountClient.GetAccountGroupAsync(request);
                if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    if (response.AccountGroupRequest !=null && response.AccountGroupRequest.Count>0)
                    {
                        return Ok(response.AccountGroupRequest);
                    }
                    else
                    {
                        return StatusCode(404, "Account Groups are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
                [HttpPost]
        [Route("accountgroup/getdetails")]
        public async Task<IActionResult> GetAccountGroupDetails(AccountBusinessService.AccountGroupDetailsRequest request)
        { 
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                AccountBusinessService.AccountGroupDetailsDataList response= await _accountClient.GetAccountGroupDetailAsync(request);
                if (response !=null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    if (response.AccountGroupDetail !=null && response.AccountGroupDetail.Count>0)
                    {
                        return Ok(response.AccountGroupDetail);
                    }
                    else
                    {
                        return StatusCode(404, "Account Group details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        // End Account Group

        // Begin Account Role        
        [HttpPost]
        [Route("addroles")]
        public async Task<IActionResult> AddRoles(AccountBusinessService.AccountRoleRequest request)
        {
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                AccountBusinessService.AccountRoleResponse response = await _accountClient.AddRolesAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("deleteroles")]
        public async Task<IActionResult> RemoveRoles(AccountBusinessService.AccountRoleDeleteRequest request)
        {
            try
            {
                // Validation  
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                AccountBusinessService.AccountRoleResponse response = await _accountClient.RemoveRolesAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("getroles")]
        public async Task<IActionResult> GetRoles(AccountBusinessService.AccountRoleDeleteRequest request)
        {
            try
            {
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                AccountBusinessService.AccountRoles response = await _accountClient.GetRolesAsync(request);
                if (response != null && response.Code==AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.Roles);
                }
                else if (response != null && response .Code==AccountBusinessService.Responcecode.Failed 
                    && response.Message=="Please provide accountid and organizationid to get roles details.")
                {
                    return StatusCode(400, "Please provide accountid and organizationid to get roles details.");
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account roles with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500,  ex.Message + " " + ex.StackTrace);
            }
        }
        // End Account Role

    }
}
