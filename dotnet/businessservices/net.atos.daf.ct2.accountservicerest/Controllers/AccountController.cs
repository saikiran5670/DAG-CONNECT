using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.accountservicerest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AccountComponent.IAccountManager accountmanager;
        private readonly Preference.IPreferenceManager preferencemanager;
        private readonly Group.IGroupManager groupmanager;
        private readonly IAuditTraillib auditlog;
        private readonly EntityMapper _mapper;
        public AccountController(ILogger<AccountController> logger, AccountComponent.IAccountManager _accountmanager, Preference.IPreferenceManager _preferencemanager, Group.IGroupManager _groupmanager, IAuditTraillib _auditlog)
        {
            _logger = logger;
            accountmanager = _accountmanager;
            preferencemanager = _preferencemanager;
            groupmanager = _groupmanager;
            auditlog = _auditlog;
            _mapper = new EntityMapper();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(AccountRequest request)
        {
            try
            {
                // Validation 
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName)) || (string.IsNullOrEmpty(request.LastName)))
                {
                    return StatusCode(400, "The EmailId address, first name, last name is required.");
                }
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account = _mapper.ToAccountEntity(request);
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.StartDate = DateTime.Now;
                account.isDuplicate = false;
                account.isError = false;
                account.EndDate = null;
                account.Password = request.Password;
                account = await accountmanager.Create(account);
                if (account.isDuplicate)
                {
                    return StatusCode(404, "The EmailId address is duplicate, please provide unique email address.");
                }
                else if (account.isError)
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else
                {
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Account Create", 1, 2, account.EmailId);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Create : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(AccountRequest request)
        {
            try
            {
                // Validation 
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName)) || (string.IsNullOrEmpty(request.LastName)))
                {
                    return StatusCode(400, "The EmailId address, first name, last name is required.");
                }
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account = _mapper.ToAccountEntity(request);
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account.isDuplicate = false;
                account.isError = false;
                account = await accountmanager.Update(account);
                if (account.isDuplicate)
                {
                    return StatusCode(400, "The Provided account is duplicate, please provide unique email address.");
                }
                else if (account.isError)
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else
                {
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Account Updated", 1, 2, account.EmailId);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Update : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(AccountRequest request)
        {
            try
            {
                // Validation                 
                if ((string.IsNullOrEmpty(request.EmailId)) || (Convert.ToInt32(request.Id) <=0 ) || (Convert.ToInt32(request.Organization_Id) <=0 ))
                {
                    return StatusCode(400, "The Email address, account id and organization id is required.");
                }
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account = _mapper.ToAccountEntity(request);
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.StartDate = DateTime.Now;
                account.EndDate = null;                
                var result = await accountmanager.Delete(account);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Service", "Account Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Account Delete", 1, 2, account.Id.ToString());
                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword(AccountRequest request)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account = _mapper.ToAccountEntity(request);
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                // Validation 
                if (string.IsNullOrEmpty(account.EmailId) || string.IsNullOrEmpty(account.Password) || (account.Id <= 0))
                {
                    return StatusCode(404, "The Email address, and account id and password is required.");
                }
                var result = await accountmanager.ChangePassword(account);
                if (result)
                {
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Account Delete", 1, 2, account.Id.ToString());
                    return Ok(account);
                }
                else
                {
                    return StatusCode(500, "There is some issues in changing password, Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get(AccountComponent.entity.AccountFilter accountFilter)
        {
            try
            {
                accountFilter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                // Validation 
                if (string.IsNullOrEmpty(accountFilter.Email) && string.IsNullOrEmpty(accountFilter.Name)
                    && (accountFilter.Id <= 0) && (accountFilter.OrganizationId <= 0) && (string.IsNullOrEmpty(accountFilter.AccountIds)))
                {
                    return StatusCode(404, "The get parameters for account is required (one of them).");
                }
                var result = await accountmanager.Get(accountFilter);
                _logger.LogInformation("Account Service - Get.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("getaccountdetail")]
        public async Task<IActionResult> GetAccountDetail(AccountDetailRequest request)
        {
            try
            {
                // Validation                 
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The organization is required");
                }
                AccountComponent.entity.AccountFilter filter = new AccountComponent.entity.AccountFilter();

                List<AccountComponent.entity.Account> accounts = new List<AccountComponent.entity.Account>();
                List<int> accountIds = new List<int>();
                List<AccountDetailsResponse> response = new List<AccountDetailsResponse>();
                AccountDetailsResponse accountDetails = new AccountDetailsResponse();
                List<string> accountGroupName = null;
                if (request.GroupId > 0)
                {
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.Id = request.GroupId;
                    groupFilter.OrganizationId = filter.OrganizationId;
                    groupFilter.ObjectType = group.ObjectType.AccountGroup;
                    groupFilter.GroupRef = true;
                    var groups = groupmanager.Get(groupFilter).Result;
                    foreach (Group.Group group in groups)
                    {
                        if (group != null && group.GroupRef != null)
                        {
                            accountIds.AddRange(group.GroupRef.Select(a => a.Ref_Id).ToList());
                        }
                    }
                    if (accountIds != null && accountIds.Count > 0)
                    {
                        filter.Id = 0;
                        filter.OrganizationId = request.OrganizationId;
                        filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                        filter.AccountIds = string.Join(",", accountIds);
                        // list of account for organization 
                        accounts = accountmanager.Get(filter).Result.ToList();
                    }
                    // get all refid
                }
                else if (request.RoleId > 0)
                {
                    accountIds = accountmanager.GetRoleAccounts(request.AccountId).Result;
                    filter.Id = 0;
                    filter.OrganizationId = request.OrganizationId;
                    filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    filter.AccountIds = string.Join(",", accountIds);
                    // list of account for organization 
                    accounts = accountmanager.Get(filter).Result.ToList();
                }
                else
                {
                    filter.Id = 0;
                    if (request.AccountId > 0) filter.Id = request.AccountId;
                    filter.OrganizationId = request.OrganizationId;
                    filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    filter.AccountIds = null;
                    // list of account for organization 
                    accounts = accountmanager.Get(filter).Result.ToList();
                }

                // account group details                 
                foreach (AccountComponent.entity.Account entity in accounts)
                {
                    accountDetails = new AccountDetailsResponse();
                    accountDetails = _mapper.ToAccountDetail(entity);
                    // group filter
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.RefId = entity.Id;
                    groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    groupFilter.GroupType = Group.GroupType.Group;
                    var accountGroupList = await groupmanager.Get(groupFilter);

                    if (accountGroupList != null)
                    {
                        accountGroupName = new List<string>();
                        foreach (Group.Group aGroup in accountGroupList)
                        {
                            accountGroupName.Add(aGroup.Name);
                        }
                    }
                    if (accountGroupName != null)
                    {
                        accountDetails.AccountGroups = string.Join(",", accountGroupName);
                    }
                    // Get roles   
                    AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                    accountRole.AccountId = entity.Id;
                    accountRole.OrganizationId = request.OrganizationId;
                    var roles = await accountmanager.GetRoles(accountRole);
                    accountDetails.Roles = string.Empty;
                    if (roles != null && Convert.ToInt32(roles.Count) > 0)
                    {
                        accountDetails.Roles = string.Join(",", roles.Select(role => role.Name).ToArray());
                    }
                    // End Get Roles
                    response.Add(accountDetails);
                    _logger.LogInformation("Get account details.");
                }
                if ((response == null) && (Convert.ToInt16(response.Count) <=0))
                {
                    return StatusCode(404, "Account Details with for provided filter not available / not configured.");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        // Begin Account Preference
        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreateAccountPreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.RefId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }                
                accountpreference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToAccountPreference(request);
                preference = await preferencemanager.Create(preference );
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.RefId));
                return Ok(preference);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("preference/update")]
        public async Task<IActionResult> UpdateAccountPreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.RefId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }                
                accountpreference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToAccountPreference(request);
                preference = await preferencemanager.Update(preference );
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.RefId));
                return Ok(preference);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeleteAccountPreference(int accountId)
        {
            try
            {
                // Validation                 
                if ((accountId <= 0))
                {
                    return StatusCode(400, "The Account Id is required");
                }
                var result = await preferencemanager.Delete(accountId);
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Preference", 1, 2, Convert.ToString(accountId));                
                return Ok(result);                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
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
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = 0;
                preferenceFilter.Ref_Id = accountId;
                preferenceFilter.PreferenceType = Preference.PreferenceType.Account;
                var result = await preferencemanager.Get(preferenceFilter);
                if ( (result == null) || Convert.ToInt16(result.Count()) <=0 )
                {
                    return StatusCode(404, "Account Preference for this account is not configured.");
                }
                _logger.LogInformation("Get account preference.");
                return Ok(result);                
            }
            catch (Exception ex)
            {
                 _logger.LogError("Error in account service:get account preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        // End - Account Preference

        // Begin - AccessRelationship
        [HttpPost]
        [Route("accessrelationship/create")]
        public async Task<IActionResult> CreateAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                // Validation                 
                if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
                }
                else if ((Convert.ToChar(request.AccessRelationType)) != 'R' || ((Convert.ToChar(request.AccessRelationType))) != 'W')
                {
                    return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
                }
                AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
                accessRelationship.Id = request.Id;
                accessRelationship.AccountGroupId = request.AccountGroupId;
                accessRelationship.VehicleGroupId = request.VehicleGroupId;
                if (request.AccessRelationType == 'R')
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
                }
                if (request.AccessRelationType == 'W')
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadWrite;
                }
                accessRelationship.StartDate = DateTime.Now;
                accessRelationship.EndDate = null;
                accessRelationship = await accountmanager.CreateAccessRelationship(accessRelationship);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Access Relationship", 1, 2, Convert.ToString(accessRelationship.AccountGroupId)).Result;
                return Ok(accessRelationship);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("accessrelationship/update")]
        public async Task<IActionResult> UpdateAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                // Validation                 
                if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
                }
                else if ((Convert.ToChar(request.AccessRelationType)) != 'R' || ((Convert.ToChar(request.AccessRelationType))) != 'W')
                {
                    return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
                }
                AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
                accessRelationship.Id = request.Id;
                accessRelationship.AccountGroupId = request.AccountGroupId;
                accessRelationship.VehicleGroupId = request.VehicleGroupId;
                if (request.AccessRelationType == 'R')
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
                }
                if (request.AccessRelationType == 'W')
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadWrite;
                }
                accessRelationship.StartDate = DateTime.Now;
                accessRelationship.EndDate = null;
                accessRelationship = await accountmanager.UpdateAccessRelationship(accessRelationship);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Update Access Relationship", 1, 2, Convert.ToString(accessRelationship.AccountGroupId)).Result;
                return Ok(accessRelationship);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("accessrelationship/delete")]
        public async Task<IActionResult> DeleteAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                // Validation                 
                if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId is required");
                }
                var result = await accountmanager.DeleteAccessRelationship(request.AccountGroupId, request.VehicleGroupId);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Access Relationship", 1, 2, Convert.ToString(request.AccountGroupId)).Result;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create access relationship with exception - " + ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("accessrelationship/get")]
        public async Task<IActionResult> GetAccessRelationship(int AccountId, int AccountGroupId)
        {
            try
            {
                // Validation                 
                if ((AccountId <= 0) && (AccountGroupId <= 0))
                {
                    return StatusCode(400, "The AccountId or AccountGroupId is required");
                }
                AccountComponent.entity.AccessRelationshipFilter filter = new AccountComponent.entity.AccessRelationshipFilter();
                filter.AccountId = AccountId;
                filter.AccountGroupId = AccountGroupId;
                var accessResult = await accountmanager.GetAccessRelationship(filter);
                if ((accessResult == null) && (Convert.ToInt16(accessResult.Count()) <= 0))
                {
                    return StatusCode(404, "AccessRelationship is not configured.");
                }
                _logger.LogInformation("Get AccessRelationship.");
                return Ok(accessResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get accessrelatioship with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        // End - AccessRelationshhip

        // Begin - Account Group

        // [HttpPost]
        // [Route("AccountGroup/Create")]
        // public async Task<IActionResult> CreateAccountGroup(AccessRelationshipRequest request)
        // {
        //     try
        //     {
        //         // Validation                 
        //         if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
        //         {
        //             return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
        //         }
        //         else if ((Convert.ToChar(request.AccessRelationType)) != 'R' || ((Convert.ToChar(request.AccessRelationType))) != 'W')
        //         {
        //             return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
        //         }
        //         AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
        //         accessRelationship.Id = request.Id;
        //         accessRelationship.AccountGroupId = request.AccountGroupId;
        //         accessRelationship.VehicleGroupId = request.VehicleGroupId;
        //         if (request.AccessRelationType == 'R')
        //         {
        //             accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
        //         }
        //         if (request.AccessRelationType == 'W')
        //         {
        //             accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadWrite;
        //         }
        //         accessRelationship.StartDate = DateTime.Now;
        //         accessRelationship.EndDate = null;
        //         accessRelationship = await accountmanager.CreateAccessRelationship(accessRelationship);
        //         var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Access Relationship", 1, 2, Convert.ToString(accessRelationship.AccountGroupId)).Result;
        //         return Ok(accessRelationship);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
        //         return StatusCode(500, "Internal Server Error.");
        //     }
        // }

        // End - Account Group

    }
}
