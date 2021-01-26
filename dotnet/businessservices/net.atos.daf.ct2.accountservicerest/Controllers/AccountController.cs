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
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AccountComponent.IAccountManager accountmanager;
        private readonly Preference.IPreferenceManager preferencemanager;
        private readonly Group.IGroupManager groupmanager;
        private readonly IAuditTraillib auditlog;
        private readonly EntityMapper _mapper;

        private string FK_Constraint = "violates foreign key constraint";
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
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName)) 
                || (string.IsNullOrEmpty(request.LastName)) || (request.Organization_Id <=0 ))
                {
                    return StatusCode(400, "The EmailId address, first name, last name and organization is required.");
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
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.Id = AccountId;
                account.Organization_Id = OrganizationId;
                account.EmailId = EmailId;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = await accountmanager.Delete(account);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Service", "Account Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Account Delete", 1, 2, account.Id.ToString());
                if (result) return Ok(account);
                else return StatusCode(404, "Account not configured.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                //account = _mapper.ToAccountEntity(request);
                account.EmailId = request.EmailId;
                account.Password = request.Password;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                // Validation 
                if (string.IsNullOrEmpty(account.EmailId) || string.IsNullOrEmpty(account.Password))
                {
                    return StatusCode(404, "The Email address, and account id and password is required.");
                }
                var result = await accountmanager.ChangePassword(account);
                if (result)
                {
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Account Delete", 1, 2, account.Id.ToString());
                    return Ok("Password has been changed");
                }
                else
                {
                    return StatusCode(404, "Account not configured.");
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
        public async Task<IActionResult> Get(AccountFilterRequest request)
        {
            try
            {
                AccountComponent.AccountFilter accountFilter = new AccountComponent.AccountFilter();
                accountFilter.Id = request.Id;
                accountFilter.OrganizationId = request.OrganizationId;
                accountFilter.Email = request.Email;
                accountFilter.AccountIds = request.AccountIds;
                accountFilter.Name = request.Name;
                accountFilter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                accountFilter.AccountGroupId = request.AccountGroupId;
                // Validation 
                if (string.IsNullOrEmpty(accountFilter.Email) && string.IsNullOrEmpty(accountFilter.Name)
                    && (accountFilter.Id <= 0) && (accountFilter.OrganizationId <= 0) && (string.IsNullOrEmpty(accountFilter.AccountIds))
                    && (accountFilter.AccountGroupId <= 0)
                    )
                {
                    return StatusCode(404, "One of the parameter to filter account is required.");
                }
                // Filter for group id
                if (accountFilter.AccountGroupId > 0)
                {

                    // Get group ref   
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.Id = accountFilter.AccountGroupId;
                    groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    groupFilter.GroupType = Group.GroupType.Group;
                    
                    groupFilter.RefId = 0;
                    groupFilter.GroupIds = null;
                    groupFilter.GroupRef = true;
                    groupFilter.GroupRefCount = true;

                    var accountGroupList = await groupmanager.Get(groupFilter);
                    var group = accountGroupList.FirstOrDefault();
                    List<int> accountIds = null;
                    if (group != null && group.GroupRef != null)
                    {
                        accountIds = new List<int>();
                        foreach (Group.GroupRef account in group.GroupRef)
                        {
                            if (account.Ref_Id > 0)
                            {
                                accountIds.Add(account.Ref_Id);
                            }
                        }
                    }
                    if (accountIds != null && Convert.ToInt32(accountIds.Count()) > 0)
                    {
                        string accountIdList = string.Join(",", accountIds);
                        if (string.IsNullOrEmpty(accountFilter.AccountIds))
                        {
                            accountFilter.AccountIds = accountIdList;
                        }
                        else
                        {
                            accountFilter.AccountIds = accountFilter.AccountIds + accountIdList;
                        }
                    }
                }
                var result = await accountmanager.Get(accountFilter);
                if (Convert.ToInt32(result.Count()) <= 0)
                {
                    return StatusCode(404, "Accounts not configured.");
                }
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
                AccountComponent.AccountFilter filter = new AccountComponent.AccountFilter();

                List<AccountComponent.entity.Account> accounts = new List<AccountComponent.entity.Account>();
                List<int> accountIds = new List<int>();
                List<AccountDetailsResponse> response = new List<AccountDetailsResponse>();
                AccountDetailsResponse accountDetails = new AccountDetailsResponse();
                if (request.AccountGroupId > 0)
                {
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.Id = request.AccountGroupId;
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                    groupFilter.GroupType = Group.GroupType.Group;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
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
                    accountIds = accountmanager.GetRoleAccounts(request.RoleId).Result;
                    filter.Id = 0;
                    filter.OrganizationId = request.OrganizationId;
                    filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    filter.AccountIds = string.Join(",", accountIds);
                    // list of account for organization 
                    accounts = accountmanager.Get(filter).Result.ToList();
                }
                else if (!string.IsNullOrEmpty(request.Name))
                {
                    filter.Id = 0;
                    filter.Id = request.AccountId;
                    filter.OrganizationId = request.OrganizationId;
                    filter.Name = request.Name;
                    filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    filter.AccountIds = null;
                    // list of account for organization 
                    accounts = accountmanager.Get(filter).Result.ToList();
                }
                // Filter based on Vehicle Group Id
                else if (request.VehicleGroupId > 0)
                {
                    // Get Access Relationship
                    AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();
                    accessFilter.AccountId = 0;
                    accessFilter.AccountGroupId = 0;
                    accessFilter.VehicleGroupId = request.VehicleGroupId;
                    var accessResult = await accountmanager.GetAccessRelationship(accessFilter);
                    if (Convert.ToInt32(accessResult.Count) > 0)
                    {
                        List<int> vehicleGroupIds = new List<int>();
                        List<int> accountIdList = new List<int>();
                        vehicleGroupIds.AddRange(accessResult.Select(c => c.AccountGroupId).ToList());
                        var groupFilter = new Group.GroupFilter();
                        groupFilter.GroupIds = vehicleGroupIds;
                        groupFilter.OrganizationId = request.OrganizationId;
                        groupFilter.GroupRefCount = false;
                        groupFilter.GroupRef = true;
                        groupFilter.ObjectType = Group.ObjectType.None;
                        groupFilter.GroupType = Group.GroupType.None;
                        groupFilter.FunctionEnum = Group.FunctionEnum.None;
                        var vehicleGroups = await groupmanager.Get(groupFilter);
                        // Get group reference
                        foreach (Group.Group vGroup in vehicleGroups)
                        {
                            foreach (Group.GroupRef groupRef in vGroup.GroupRef)
                            {
                                if (groupRef.Ref_Id > 0) 
                                    accountIdList.Add(groupRef.Ref_Id);
                            }
                        }
                        if (accountIdList != null && accountIdList.Count > 0)
                        {
                            filter.Id = 0;
                            filter.OrganizationId = request.OrganizationId;
                            filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                            filter.AccountIds = string.Join(",", accountIdList);                            
                            // get accounts details
                            accounts = accountmanager.Get(filter).Result.ToList();
                        }
                    }
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
                    accountDetails.AccountGroups = new List<KeyValue>();
                    accountDetails.Roles = new List<KeyValue>();
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
                        foreach (Group.Group aGroup in accountGroupList)
                        {
                            //accountGroupName.Add(aGroup.Name);
                            accountDetails.AccountGroups.Add(new KeyValue() { Id = aGroup.Id, Name = aGroup.Name });
                        }
                    }
                    // Get roles   
                    AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                    accountRole.AccountId = entity.Id;
                    accountRole.OrganizationId = request.OrganizationId;
                    var roles = await accountmanager.GetRoles(accountRole);
                    if (roles != null && Convert.ToInt32(roles.Count) > 0)
                    {
                        foreach (AccountComponent.entity.KeyValue role in roles)
                        {
                            //accountGroupName.Add(aGroup.Name);
                            accountDetails.Roles.Add(new KeyValue() { Id = role.Id, Name = role.Name });
                        }
                    }
                    // End Get Roles
                    response.Add(accountDetails);
                    _logger.LogInformation("Get account details.");
                }
                if ((Convert.ToInt16(response.Count()) <= 0))
                {
                    return StatusCode(404, "Accounts not configured.");
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
                preference.Exists=false;
                preference = await preferencemanager.Create(preference);
                // check for exists
                if (preference.Exists)
                {
                    return StatusCode(409, "Duplicate Account Preference.");
                }
                // check for exists
                else if (preference.RefIdNotValid)
                {
                    return StatusCode(400, "The Ref_Id not valid or created.");
                }

                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.RefId));

                return Ok(preference);
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
                preference = await preferencemanager.Update(preference);
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.RefId));
                return Ok(preference);
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
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = 0;
                preferenceFilter.Ref_Id = accountId;
                preferenceFilter.PreferenceType = Preference.PreferenceType.Account;
                var result = await preferencemanager.Get(preferenceFilter);
                if ((result == null) || Convert.ToInt16(result.Count()) <= 0)
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
                else if ((!request.AccessRelationType.Equals("R")) && (!request.AccessRelationType.Equals("W")))
                {
                    return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
                }
                AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
                accessRelationship.Id = request.Id;
                accessRelationship.AccountGroupId = request.AccountGroupId;
                accessRelationship.VehicleGroupId = request.VehicleGroupId;
                if (request.AccessRelationType.Equals("R"))
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
                }
                if (request.AccessRelationType.Equals("W"))
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
        public async Task<IActionResult> UpdateAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
                }
                else if ((!request.AccessRelationType.Equals("R")) && (!request.AccessRelationType.Equals("W")))
                {
                    return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
                }
                AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
                accessRelationship.Id = request.Id;
                accessRelationship.AccountGroupId = request.AccountGroupId;
                accessRelationship.VehicleGroupId = request.VehicleGroupId;
                if (request.AccessRelationType.Equals("R"))
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
                }
                if (request.AccessRelationType.Equals("W"))
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
                if ((request.Id <= 0))
                {
                    return StatusCode(400, "The AccountGroupId,VehicleGroupId is required");
                }
                var result = await accountmanager.DeleteAccessRelationship(request.Id, request.VehicleGroupId);
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

        [HttpPost]
        [Route("accountgroup/create")]
        public async Task<IActionResult> CreateAccountGroup(AccountGroupRequest request)
        {
            try
            {
                // Validation                 
                if ((string.IsNullOrEmpty(request.Name)))
                {
                    return StatusCode(400, "The AccountGroup name is required");
                }
                Group.Group group = new Group.Group();
                group.Name = request.Name;
                group.Description = request.Description;
                group.OrganizationId = request.OrganizationId;
                group.FunctionEnum = Group.FunctionEnum.None;
                group.GroupType = Group.GroupType.Group;
                group.ObjectType = Group.ObjectType.AccountGroup;

                var result = await groupmanager.Create(group);
                // check for exists
                if (result.Exists)
                {
                    return StatusCode(409, "Duplicate Account Group.");
                }
                group.Id = result.Id;
                group.Id = result.Id;
                if (result.Id > 0 && request.Accounts != null)
                {
                    group.GroupRef = new List<Group.GroupRef>();
                    foreach (var item in request.Accounts)
                    {
                        if (item.AccountId > 0)
                            group.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.AccountId, Group_Id = group.Id });
                    }
                    if (Convert.ToInt32(group.GroupRef.Count) > 0)
                    {
                        bool AddvehicleGroupRef = await groupmanager.UpdateRef(group);
                    }
                }
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Account Group", 1, 2, Convert.ToString(group.Name));
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("accountgroup/update")]
        public async Task<IActionResult> UpdateAccountGroup(AccountGroupRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (string.IsNullOrEmpty(request.Name)))
                {
                    return StatusCode(400, "The AccountGroup name and id is required");
                }
                Group.Group group = new Group.Group();
                group.Id = request.Id;
                group.Name = request.Name;
                group.Description = request.Description;
                group.OrganizationId = request.OrganizationId;
                group.FunctionEnum = Group.FunctionEnum.None;
                group.GroupType = Group.GroupType.Group;
                group.ObjectType = Group.ObjectType.AccountGroup;
                var result = await groupmanager.Update(group);
                group.Id = result.Id;
                // check for exists
                if (result.Exists)
                {
                    return StatusCode(409, "Duplicate Account Group.");
                }
                if (result.Id > 0 && request.Accounts != null)
                {
                    group.GroupRef = new List<Group.GroupRef>();
                    foreach (var item in request.Accounts)
                    {
                        if (item.AccountId > 0)
                            group.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.AccountId, Group_Id = group.Id });
                    }
                    if (Convert.ToInt32(group.GroupRef.Count) > 0)
                    {
                        bool AddvehicleGroupRef = await groupmanager.UpdateRef(group);
                    }
                }
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Account Group", 1, 2, Convert.ToString(group.Name));
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
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
                bool result = await groupmanager.Delete(id);
                var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Create Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Account Group ", 1, 2, Convert.ToString(id));
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group :DeleteGroup with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("accountgroup/addaccounts")]
        public async Task<IActionResult> AddAccountsToGroup(AccountGroupAccount request)
        {
            try
            {
                // Validation  
                if (request == null)
                {
                    return StatusCode(400, "The AccountGroup account is required");
                }
                bool result = false;
                List<Group.GroupRef> groupRef = null;
                if (request != null && request.Accounts != null)
                {
                    groupRef = new List<Group.GroupRef>();
                    foreach (var groupref in request.Accounts)
                    {
                        groupRef.Add(new Group.GroupRef() { Group_Id = groupref.AccountGroupId, Ref_Id = groupref.AccountId });
                    }
                    result = await groupmanager.AddRefToGroups(groupRef);
                }
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Acccount Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Account Group ", 1, 2, "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group :DeleteGroup with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPut]
        [Route("accountgroup/deleteaccounts")]
        public async Task<IActionResult> DeleteAccountFromGroup(int id)
        {
            try
            {
                // Validation  
                if (id <= 0)
                {
                    return StatusCode(400, "The AccountGroup Id is required");
                }
                bool result = false;
                result = await groupmanager.RemoveRef(id);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Acccount Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Account Group References", 1, 2, "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group reference :DeleteAccountsGroupReference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("accountgroup/get")]
        public async Task<IActionResult> GetAccountGroup(AccountGroupFilterRequest request)
        {
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                Group.GroupFilter group = new Group.GroupFilter();
                group.Id = request.AccountGroupId;
                group.OrganizationId = request.OrganizationId;
                group.GroupRef = request.Accounts;
                group.GroupRefCount = request.AccountCount;
                // filter based on account id
                group.RefId = request.AccountId;

                group.FunctionEnum = Group.FunctionEnum.None;
                group.ObjectType = Group.ObjectType.AccountGroup;
                group.GroupType = Group.GroupType.Group;

                var groups = await groupmanager.Get(group);
                _logger.LogInformation("Get account group.");
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("accountgroup/getdetails")]
        public async Task<IActionResult> GetAccountGroupDetails(AccountGroupFilterRequest request)
        {
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                Group.GroupFilter groupFilter = new Group.GroupFilter();
                List<AccountGroupDetailRequest> response = new List<AccountGroupDetailRequest>();
                AccountGroupDetailRequest accountDetail = null;
                groupFilter.OrganizationId = request.OrganizationId;
                groupFilter.GroupType = Group.GroupType.Group;
                groupFilter.FunctionEnum = Group.FunctionEnum.None;
                groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                groupFilter.GroupRefCount = true;

                // all account group of organization with account count
                var groups = await groupmanager.Get(groupFilter);
                // get access relationship 
                AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();

                foreach (Group.Group group in groups)
                {
                    accountDetail = new AccountGroupDetailRequest();
                    accountDetail.Id = group.Id;
                    accountDetail.Name = group.Name;
                    accountDetail.AccountCount = group.GroupRefCount;
                    accessFilter.AccountGroupId = group.Id;
                    var accessList = accountmanager.GetAccessRelationship(accessFilter).Result;
                    List<Int32> groupId = new List<int>();
                    accountDetail.VehicleCount = 0;

                    // vehicle group 
                    if (Convert.ToInt32(accessList.Count) > 0)
                    {
                        groupId.AddRange(accessList.Select(c => c.VehicleGroupId).ToList());
                        groupFilter = new Group.GroupFilter();
                        groupFilter.GroupIds = groupId;
                        groupFilter.GroupRefCount = true;
                        groupFilter.ObjectType = Group.ObjectType.None;
                        groupFilter.GroupType = Group.GroupType.None;
                        groupFilter.FunctionEnum = Group.FunctionEnum.None;
                        var vehicleGroups = groupmanager.Get(groupFilter).Result;
                        Int32 count = 0;
                        // Get vehicles count
                        foreach (Group.Group vGroup in vehicleGroups)
                        {
                            count = count + vGroup.GroupRefCount;
                        }
                        accountDetail.VehicleCount = count;
                    }
                    response.Add(accountDetail);
                }
                _logger.LogInformation("Get account group details.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("addroles")]
        public async Task<IActionResult> AddRoles(AccountRoleRequest request)
        {
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                bool result = false;
                AccountComponent.entity.AccountRole role = new AccountComponent.entity.AccountRole();
                //AccountRoleResponse response = new AccountRoleResponse();
                if ((request != null) && (request.Roles != null) && Convert.ToInt16(request.Roles.Count) > 0)
                {
                    role.OrganizationId = request.OrganizationId;
                    role.AccountId = request.AccountId;
                    role.RoleIds = new List<int>();
                    foreach (int roleid in request.Roles)
                    {
                        role.RoleIds.Add(roleid);
                    }
                    role.StartDate = DateTime.UtcNow;
                    role.EndDate = null;
                    result = await accountmanager.AddRole(role);
                }
                _logger.LogInformation("Get account group details.");
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Add Roles to accoount", 1, 2, Convert.ToString(request.AccountId)).Result;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("deleteroles")]
        public async Task<IActionResult> RemoveRoles(AccountRoleRequest request)
        {
            try
            {
                // Validation  
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                accountRole.OrganizationId = request.OrganizationId;
                accountRole.AccountId = request.AccountId;
                var result = await accountmanager.RemoveRole(accountRole);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Remove Roles from accoount id", 1, 2, Convert.ToString(request.AccountId)).Result;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpPost]
        [Route("getroles")]
        public async Task<IActionResult> GetRoles(AccountRoleRequest request)
        {
            try
            {
                // Validation 
                List<AccountRoleResponse> response = new List<AccountRoleResponse>();
                AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                accountRole.OrganizationId = request.OrganizationId;
                accountRole.AccountId = request.AccountId;
                var roles = await accountmanager.GetRoles(accountRole);
                foreach (AccountComponent.entity.KeyValue role in roles)
                {
                    //response.Roles = new NameIdResponse();
                    response.Add(new AccountRoleResponse() { Id = role.Id, Name = role.Name });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account roles with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        // End - Account Group

    }
}
