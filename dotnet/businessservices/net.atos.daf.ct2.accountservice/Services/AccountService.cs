using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.accountservice.Entity;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.vehicle;
using Newtonsoft.Json;
using AccountComponent = net.atos.daf.ct2.account;
using Group = net.atos.daf.ct2.group;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using Preference = net.atos.daf.ct2.accountpreference;
using vehicleEntity = net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.accountservice
{
    public class AccountManagementService : AccountService.AccountServiceBase
    {
        // private readonly ILogger<AccountManagementService> _logger;
        private readonly AccountComponent.IAccountManager _accountmanager;
        private readonly Preference.IPreferenceManager _preferencemanager;
        private readonly Group.IGroupManager _groupManager;
        private readonly Mapper _mapper;
        private readonly IVehicleManager _vehicelManager;
        private readonly ILog _logger;

        private readonly AccountComponent.IAccountIdentityManager _accountIdentityManager;

        #region Constructor
        public AccountManagementService(AccountComponent.IAccountManager Accountmanager, Preference.IPreferenceManager Preferencemanager, Group.IGroupManager Groupmanager, AccountComponent.IAccountIdentityManager AccountIdentityManager, IVehicleManager VehicelManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._accountmanager = Accountmanager;
            _preferencemanager = Preferencemanager;
            this._groupManager = Groupmanager;
            this._accountIdentityManager = AccountIdentityManager;
            _mapper = new Mapper();
            _vehicelManager = VehicelManager;
        }
        #endregion

        #region Identity

        public override Task<AccountIdentityResponse> Auth(IdentityRequest request, ServerCallContext context)
        {
            AccountIdentityResponse response = new AccountIdentityResponse();
            try
            {
                Identity account = new Identity();
                account.UserName = request.UserName.Trim();
                account.Password = request.Password;
                AccountIdentity accIdentity = _accountIdentityManager.Login(account).Result;
                if (accIdentity != null && accIdentity.StatusCode == 5)
                {
                    return Task.FromResult(new AccountIdentityResponse
                    {
                        //Account not present  in IDP or IDP related error
                        Code = Responcecode.Forbidden,
                        Message = "System accounts are restricted on the portal.",
                        TokenIdentifier = string.Empty,
                    });
                }
                else if (accIdentity != null && (!string.IsNullOrEmpty(accIdentity.TokenIdentifier)))
                {
                    _logger.Info("account is Authenticated");
                    response.TokenIdentifier = accIdentity.TokenIdentifier;
                    if (accIdentity.AccountInfo != null)
                    {
                        response.AccountInfo = _mapper.ToAccount(accIdentity.AccountInfo);
                    }
                    if (accIdentity.AccountOrganization != null && accIdentity.AccountOrganization.Count > 0)
                    {
                        AccountIdentityOrg acctOrganization = new AccountIdentityOrg();
                        foreach (var accOrg in accIdentity.AccountOrganization)
                        {
                            acctOrganization = new AccountIdentityOrg();
                            acctOrganization.Id = accOrg.Id;
                            acctOrganization.Name = accOrg.Name;
                            response.AccOrganization.Add(acctOrganization);
                        }
                    }
                    if (accIdentity.AccountRole != null && accIdentity.AccountRole.Count > 0)
                    {
                        AccountIdentityRole accRole = new AccountIdentityRole();
                        foreach (var accr in accIdentity.AccountRole)
                        {
                            accRole = new AccountIdentityRole();
                            accRole.Id = accr.Id;
                            accRole.Name = accr.Name;
                            accRole.OrganizationId = accr.Organization_Id;
                            response.AccountRole.Add(accRole);
                        }
                    }
                    return Task.FromResult(response);
                }
                else if (accIdentity != null && string.IsNullOrEmpty(accIdentity.TokenIdentifier))
                {
                    return Task.FromResult(new AccountIdentityResponse
                    {
                        //Account not present  in IDP or IDP related error
                        Code = (Responcecode)accIdentity.StatusCode,
                        Message = accIdentity.ErrorMessage,
                        ResetPasswordExpiryResponse = new ResetPasswordExpiryResponse { ProcessToken = accIdentity.Token != null ? accIdentity.Token?.ProcessToken : string.Empty }
                    });
                }
                else
                {
                    return Task.FromResult(new AccountIdentityResponse
                    {
                        //Account not present  in IDP or IDP related error
                        Code = Responcecode.Failed,
                        Message = "Account is not configured.",
                        TokenIdentifier = string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return Task.FromResult(new AccountIdentityResponse
                {
                    Code = Responcecode.Failed,
                    Message = " Authentication is failed due to - " + ex.ToString(),
                    TokenIdentifier = string.Empty,
                });
            }
        }
        public override Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
        {
            LogoutResponse response = new LogoutResponse();
            try
            {
                bool result = _accountIdentityManager.LogoutByTokenId(request.TokenId).Result;
                if (result)
                {
                    _logger.Info("account is logged out");
                    response.Success = true;
                }
                else
                {
                    _logger.Info("account is logged out");
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                response.Success = false;
            }
            return Task.FromResult(response);
        }

        #endregion

        #region Account
        public override async Task<AccountData> Create(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account = _mapper.ToAccountEntity(request);
                account = await _accountmanager.Create(account);
                // response 
                AccountData response = new AccountData();
                if (account.IsDuplicate || account.IsDuplicateInOrg)
                {
                    response.Message = "The duplicate account.";
                    response.Code = Responcecode.Conflict;
                    response.Account = _mapper.ToAccount(account);
                }
                else if (account.IsError)
                {
                    response.Message = "There is an error creating account.";
                    response.Code = Responcecode.Failed;
                }
                //Removed because account creation should not depend on email functionality failure
                //else if (account.IsErrorInEmail)
                //{
                //    response.Message = "There is an error while sending account confirmation email to the account user.";
                //    response.Code = Responcecode.Failed;
                //    response.Account = _mapper.ToAccount(account);
                //}
                else
                {
                    response.Code = Responcecode.Success;
                    request.Id = account.Id;
                    response.Message = "Created";
                    request.Id = account.Id;
                    response.Account = request;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountData
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Failed due to - " + ex.Message,
                    Account = null
                });
            }
        }
        public override async Task<AccountData> Update(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account = _mapper.ToAccountEntity(request);
                account.AccountType = (AccountComponent.ENUM.AccountType)request?.Type[0];
                account = await _accountmanager.Update(account);
                // response 
                AccountData response = new AccountData();
                response.Code = Responcecode.Success;
                response.Message = "Updated";
                response.Account = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountData
                {
                    Code = Responcecode.Failed,
                    Message = "Account Updation Failed due to - " + ex.Message,
                    Account = null
                });
            }
        }
        public override async Task<AccountResponse> Delete(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                account.Organization_Id = request.OrganizationId;
                account.StartDate = null;
                account.EndDate = null;
                if (request.StartDate > 0) account.StartDate = request.StartDate;
                if (request.EndDate > 0) account.EndDate = request.EndDate;
                var result = await _accountmanager.Delete(account);
                // response 
                AccountResponse response = new AccountResponse();
                response.Code = Responcecode.Success;
                response.Message = "Delete";
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Deletion Failed due to - " + ex.Message
                });
            }
        }
        public override async Task<AccountResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.EmailId = request.EmailId;
                account.Password = request.Password;
                account.Organization_Id = request.OrgId;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                var identityResult = await _accountmanager.ChangePassword(account);
                // response 
                AccountResponse response = new AccountResponse();
                if (identityResult.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Password has been changed.";
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    IdentityResponse responseContent = null;
                    try
                    {
                        responseContent = JsonConvert.DeserializeObject<IdentityResponse>(identityResult.Result.ToString());
                    }
                    catch (Exception)
                    { }
                    if (responseContent != null && responseContent.Error.Equals("invalidPasswordHistoryMessage"))
                    {
                        response.Code = Responcecode.BadRequest;
                        response.Message = "Password must not be equal to any of last 6 passwords.";
                    }
                    else if (responseContent != null && responseContent.Error.Equals("InValidPassword"))
                    {
                        response.Code = Responcecode.BadRequest;
                        response.Message = responseContent.Error_Description;
                    }
                    else
                    {
                        response.Code = Responcecode.Failed;
                    }
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Account not configured.";
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    response.Code = Responcecode.Forbidden;
                    response.Message = "Request does not satisfy minimum password age criteria.";
                }
                else
                {
                    response.Code = Responcecode.Failed;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Change Password failed due to with reason : " + ex.Message
                });
            }
        }
        public override async Task<AccountDataList> Get(AccountFilter request, ServerCallContext context)
        {
            try
            {
                AccountComponent.AccountFilter filter = new AccountComponent.AccountFilter();
                filter.Id = request.Id;
                filter.OrganizationId = request.OrganizationId;
                filter.Email = request.Email;
                filter.AccountIds = request.AccountIds;
                filter.Name = request.Name;
                filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                filter.AccountGroupId = request.AccountGroupId;

                // Filter for group id
                if (filter.AccountGroupId > 0)
                {

                    // Get group ref   
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.Id = filter.AccountGroupId;
                    groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    groupFilter.GroupType = Group.GroupType.Group;
                    groupFilter.RefId = 0;
                    groupFilter.GroupIds = null;
                    groupFilter.GroupRef = true;
                    groupFilter.GroupRefCount = false;
                    // get account group accounts

                    var accountGroupList = await _groupManager.Get(groupFilter);
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
                        if (string.IsNullOrEmpty(filter.AccountIds))
                        {
                            filter.AccountIds = accountIdList;
                        }
                        else
                        {
                            filter.AccountIds = filter.AccountIds + accountIdList;
                        }
                    }
                }

                var result = await _accountmanager.Get(filter);
                _logger.Info("Account Service - Get.");
                // response 
                AccountDataList response = new AccountDataList();
                foreach (AccountComponent.entity.Account entity in result)
                {
                    response.Accounts.Add(_mapper.ToAccount(entity));
                }
                response.Code = Responcecode.Success;
                response.Message = "Get";
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountDataList
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<AccountOrganizationResponse> AddAccountToOrg(AccountOrganization request, ServerCallContext context)
        {
            try
            {
                var accountType = context.RequestHeaders.Get("account_type").Value;

                Account account = new Account();
                AccountOrganizationResponse response = new AccountOrganizationResponse();
                account.Id = request.AccountId;
                account.AccountType = (AccountComponent.ENUM.AccountType)Convert.ToChar(accountType);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = null;
                account.EndDate = null;
                if (request.StartDate > 0) account.StartDate = request.StartDate;
                if (request.StartDate > 0) account.StartDate = request.StartDate;
                var result = await _accountmanager.AddAccountToOrg(account);

                if (result == null)
                {
                    response.Code = Responcecode.Forbidden;
                    response.Message = "Multiple organizations cannot be linked to a System Account.";
                }
                else
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Account Added to organization.";
                    response.AccountOrgId = account.Id;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountOrganizationResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Deletion Failed due to - " + ex.Message
                });
            }
        }

        public override async Task<AccountDetailsResponse> GetAccountDetail(AccountGroupDetailsRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.AccountFilter filter = new AccountComponent.AccountFilter();
                List<AccountComponent.entity.Account> accounts = new List<AccountComponent.entity.Account>();
                List<int> accountIds = new List<int>();
                AccountDetailsResponse response = new AccountDetailsResponse();
                AccountDetails accountDetails = new AccountDetails();
                if (request.AccountGroupId > 0)
                {
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.Id = request.AccountGroupId;
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                    groupFilter.GroupType = Group.GroupType.None;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    groupFilter.GroupRef = true;
                    // get account group accounts
                    var groups = _groupManager.Get(groupFilter).Result;
                    foreach (Group.Group group in groups)
                    {
                        // check for dynamic group 
                        if (group.GroupType == Group.GroupType.Dynamic && group.ObjectType == Group.ObjectType.AccountGroup)
                        {
                            filter.Id = 0;
                            filter.OrganizationId = request.OrganizationId;
                            filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                            filter.AccountIds = string.Empty;
                            accounts = _accountmanager.Get(filter).Result.ToList();
                        }
                        else
                        {
                            if (group != null && group.GroupRef != null)
                            {

                                accountIds.AddRange(group.GroupRef.Select(a => a.Ref_Id).ToList());

                            }
                        }
                    }
                    if (accountIds != null && accountIds.Count > 0)
                    {
                        filter.Id = 0;
                        filter.OrganizationId = request.OrganizationId;
                        filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                        filter.AccountIds = string.Join(",", accountIds);
                        // list of account for organization 
                        accounts.AddRange(_accountmanager.Get(filter).Result.ToList());
                    }
                    // get all refid
                }
                else if (request.RoleId > 0)
                {
                    accountIds = _accountmanager.GetRoleAccounts(request.RoleId).Result;
                    filter.Id = 0;
                    filter.OrganizationId = request.OrganizationId;
                    filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    filter.AccountIds = string.Join(",", accountIds);
                    // list of account for organization 
                    accounts = _accountmanager.Get(filter).Result.ToList();
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
                    accounts = _accountmanager.Get(filter).Result.ToList();
                }
                // Filter based on Vehicle Group Id
                else if (request.VehicleGroupId > 0)
                {
                    // Get Access Relationship
                    AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();
                    accessFilter.AccountId = 0;
                    accessFilter.AccountGroupId = 0;
                    accessFilter.VehicleGroupId = request.VehicleGroupId;
                    // get account group and vehicle group access relationship.
                    var accessResult = await _accountmanager.GetAccessRelationship(accessFilter);
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
                        var vehicleGroups = await _groupManager.Get(groupFilter);
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
                            accounts = _accountmanager.Get(filter).Result.ToList();
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
                    accounts = _accountmanager.Get(filter).Result.ToList();
                }
                // account group details                 
                foreach (AccountComponent.entity.Account entity in accounts)
                {
                    accountDetails = new AccountDetails();
                    accountDetails.Account = _mapper.ToAccountDetail(entity);
                    //accountDetails.AccountGroups = new Google.Protobuf.Collections.RepeatedField<KeyValue>();
                    //accountDetails.Roles = new Google.Protobuf.Collections.RepeatedField<KeyValue>();
                    // group filter
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.RefId = entity.Id;
                    groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    groupFilter.GroupType = Group.GroupType.Group;
                    var accountGroupList = await _groupManager.Get(groupFilter);
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
                    var roles = await _accountmanager.GetRoles(accountRole);
                    if (roles != null && Convert.ToInt32(roles.Count) > 0)
                    {
                        foreach (AccountComponent.entity.KeyValue role in roles)
                        {
                            //accountGroupName.Add(aGroup.Name);
                            accountDetails.Roles.Add(new KeyValue() { Id = role.Id, Name = role.Name });
                        }
                    }
                    // End Get Roles
                    response.AccountDetails.Add(accountDetails);
                }
                _logger.Info("Get account details.");
                response.Code = Responcecode.Success;
                response.Message = "Get";
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountDetailsResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<ResetPasswordResponse> ResetPasswordInitiate(ResetPasswordInitiateRequest request, ServerCallContext context)
        {
            try
            {
                var identityResult = await _accountmanager.ResetPasswordInitiate(request.EmailId);

                ResetPasswordResponse response = new ResetPasswordResponse();
                if (identityResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Reset password process is initiated.";
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Account not configured";
                }
                else
                {
                    response.Code = Responcecode.Failed;
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ResetPasswordResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Password Reset failed due to the reason : " + ex.Message
                });
            }
        }

        public override async Task<ResetPasswordResponse> GetResetPasswordTokenStatus(GetResetPasswordTokenStatusRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _accountmanager.GetResetPasswordTokenStatus(new Guid(request.ProcessToken));

                ResetPasswordResponse response = new ResetPasswordResponse();
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Activation link is valid.";
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Email activation link is either Expired or Invalidated.";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ResetPasswordResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Get Reset Password status failed due to the reason : " + ex.Message
                });
            }
        }

        public override async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.ProcessToken = new Guid(request.ProcessToken);
                account.Password = request.Password;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                var identityResult = await _accountmanager.ResetPassword(account);

                ResetPasswordResponse response = new ResetPasswordResponse();
                if (identityResult.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Password has been reset successfully.";
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    IdentityResponse responseContent = null;
                    try
                    {
                        responseContent = JsonConvert.DeserializeObject<IdentityResponse>(identityResult.Result.ToString());
                    }
                    catch (Exception)
                    { }

                    if (responseContent != null && responseContent.Error.Equals("invalidPasswordHistoryMessage"))
                    {
                        response.Code = Responcecode.BadRequest;
                        response.Message = "Password must not be equal to any of last 6 passwords.";
                    }
                    else if (responseContent != null && responseContent.Error.Equals("InValidPassword"))
                    {
                        response.Code = Responcecode.BadRequest;
                        response.Message = responseContent.Error_Description;
                    }
                    else
                    {
                        response.Code = Responcecode.Failed;
                    }
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Email activation link is either Expired or Invalidated.";
                }
                else
                {
                    response.Code = Responcecode.Failed;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ResetPasswordResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Password Reset failed due to the reason : " + ex.Message
                });
            }
        }
        public override async Task<ResetPasswordResponse> ResetPasswordInvalidate(ResetPasswordInvalidateRequest request, ServerCallContext context)
        {
            try
            {
                var identityResult = await _accountmanager.ResetPasswordInvalidate(new Guid(request.ResetToken));

                ResetPasswordResponse response = new ResetPasswordResponse();
                if (identityResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Reset token has been invalidated.";
                }
                else if (identityResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Email activation link is either Expired or already Invalidated.";
                }
                else
                {
                    response.Code = Responcecode.Failed;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ResetPasswordResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Reset Password Invalidate failed due to the reason : " + ex.Message
                });
            }
        }

        public override async Task<MenuFeatureResponse> GetMenuFeatures(MenuFeatureRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _accountmanager.GetMenuFeatures(new MenuFeatureRquest()
                {
                    AccountId = request.AccountId,
                    ContextOrgId = request.ContextOrgId,
                    LanguageCode = request.LanguageCode,
                    OrganizationId = request.OrganizationId,
                    RoleId = request.RoleId
                });

                MenuFeatureResponse response = new MenuFeatureResponse();
                if (result.Count() > 0)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Menu items and features fetched successfully.";
                    response.MenuFeatures = MapMenuFeatureDtoToList(result.ToList());
                }
                else
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "No menu items and features found for the provided account details.";
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new MenuFeatureResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get Menu Features failed due to the reason : " + ex.Message
                });
            }
        }
        // End Account
        #endregion

        #region Account Blob
        public override async Task<AccountBlobResponse> SaveProfilePicture(AccountBlobRequest request, ServerCallContext context)
        {
            try
            {
                AccountBlobResponse response = new AccountBlobResponse();
                AccountComponent.entity.AccountBlob accountBlob = new AccountComponent.entity.AccountBlob();
                accountBlob = _mapper.AccountBlob(request);
                var accountResponse = await _accountmanager.CreateBlob(accountBlob);
                request = _mapper.AccountBlob(accountResponse);
                response.BlobId = request.Id;
                // response 
                response.Message = "Account Profile Picture Updated.";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountBlobResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Blob Creation Failed due to - " + ex.Message
                });
            }
        }
        public override async Task<AccountBlobResponse> GetProfilePicture(IdRequest request, ServerCallContext context)
        {
            try
            {
                AccountBlobResponse response = new AccountBlobResponse();
                AccountComponent.entity.AccountBlob accountBlob = new AccountComponent.entity.AccountBlob();
                var accountResponse = await _accountmanager.GetBlob(request.Id);
                if (accountResponse == null)
                {
                    response.BlobId = request.Id;
                    response.Message = "Not Found.";
                    response.Code = Responcecode.NotFound;
                    return await Task.FromResult(response);
                }
                response.BlobId = accountResponse.Id;
                response.Image = ByteString.CopyFrom(accountResponse.Image);
                // response 
                response.Message = "Account Profile Picture Updated.";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountBlobResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Blob Creation Failed due to - " + ex.Message
                });
            }
        }

        #endregion

        #region VehicleAccount AccessRelationship

        public override async Task<ServiceResponse> CreateVehicleAccessRelationship(VehicleAccessRelationship request, ServerCallContext context)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {

                int vehicleGroupId = request.Id;
                string groupName;
                Group.Group group;
                if (!request.IsGroup)
                {
                    // create vehicle group with vehicle                    
                    groupName = string.Format("VehicleGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                    group = new Group.Group(Group.GroupType.Single, Group.ObjectType.VehicleGroup, null,
                                                    Group.FunctionEnum.None, request.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                    group = await _groupManager.Create(group);
                    vehicleGroupId = group.Id;
                }
                if (vehicleGroupId > 0)
                {
                    foreach (var account in request.AccountsAccountGroup)
                    {
                        int accountGroupId;
                        // create group type single
                        if (!account.IsGroup && account.Id > 0)
                        {
                            // create vehicle group with vehicle                            
                            groupName = string.Format("AccountGroup_{0}_{1}", request.OrganizationId.ToString(), account.Id.ToString());
                            if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                            group = new Group.Group(Group.GroupType.Single, Group.ObjectType.AccountGroup, null,
                                                          Group.FunctionEnum.None, account.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                            group = await _groupManager.Create(group);
                            accountGroupId = group.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var result = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                        else if (account.Id > 0)
                        {
                            accountGroupId = account.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var result = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                    }
                }
                response.Code = Responcecode.Success;
                response.Message = "Created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ServiceResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<ServiceResponse> UpdateVehicleAccessRelationship(VehicleAccessRelationship request, ServerCallContext context)
        {
            var response = new ServiceResponse();
            bool result = true;
            try
            {

                int vehicleGroupId = request.Id;
                string groupName;
                Group.Group group;
                // check for vehicle group
                if (!request.IsGroup)
                {
                    // create vehicle group with vehicle                    
                    groupName = string.Format("VehicleGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                    group = new Group.Group(Group.GroupType.Single, Group.ObjectType.VehicleGroup, null,
                                                    Group.FunctionEnum.None, request.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                    group = await _groupManager.Create(group);
                    vehicleGroupId = group.Id;
                }
                // delete access relatioship for vehicle or vehicle group
                if (request.OrganizationId > 0 && request.Id > 0)
                {
                    result = await _accountmanager.DeleteVehicleAccessRelationship(request.OrganizationId, vehicleGroupId, true);
                }
                if (result)
                {
                    foreach (var account in request.AccountsAccountGroup)
                    {
                        int accountGroupId;
                        // create group type single
                        if (!account.IsGroup && account.Id > 0)
                        {
                            // create vehicle group with vehicle                            
                            groupName = string.Format("AccountGroup_{0}_{1}", request.OrganizationId.ToString(), account.Id.ToString());
                            if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                            group = new Group.Group(Group.GroupType.Single, Group.ObjectType.AccountGroup, null,
                                                          Group.FunctionEnum.None, account.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                            group = await _groupManager.Create(group);
                            accountGroupId = group.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var accessRelationshipResult = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                        else if (account.Id > 0)
                        {
                            accountGroupId = account.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var accessRelationshipResult = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                    }
                }
                response.Code = Responcecode.Success;
                response.Message = "Created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ServiceResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<ServiceResponse> DeleteVehicleAccessRelationship(DeleteAccessRelationRequest request, ServerCallContext context)
        {
            bool result = true;
            var response = new ServiceResponse();
            try
            {

                // check for vehicle group
                int vehicleGroupId = request.Id;
                if (!request.IsGroup)
                {
                    // create vehicle group with vehicle                    
                    string groupName = string.Format("VehicleGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                    Group.Group group = new Group.Group(Group.GroupType.Single, Group.ObjectType.VehicleGroup, null,
                                        Group.FunctionEnum.None, request.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                    group = await _groupManager.Create(group);
                    vehicleGroupId = group.Id;
                }
                // delete access relatioship for vehicle or vehicle group
                if (request.OrganizationId > 0 && request.Id > 0)
                {
                    result = await _accountmanager.DeleteVehicleAccessRelationship(request.OrganizationId, vehicleGroupId, true);
                }
                if (result)
                {
                    response.Code = Responcecode.Success;
                }
                else
                {
                    response.Code = Responcecode.Failed;
                }
                response.Message = "Deleted";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ServiceResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<ServiceResponse> CreateAccountAccessRelationship(AccountAccessRelationship request, ServerCallContext context)
        {
            var response = new ServiceResponse();
            try
            {

                // long CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                int accountGroupId = request.Id;
                string groupName;
                Group.Group group;
                if (!request.IsGroup && request.Id > 0)
                {
                    // create vehicle group with vehicle                    
                    groupName = string.Format("AccountGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                    group = new Group.Group(Group.GroupType.Single, Group.ObjectType.AccountGroup, null,
                                                    Group.FunctionEnum.None, request.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                    group = await _groupManager.Create(group);
                    accountGroupId = group.Id;
                }
                if (accountGroupId > 0)
                {
                    foreach (var vehicle in request.VehiclesVehicleGroups)
                    {
                        int vehicleGroupId;
                        // create group type single
                        if (!vehicle.IsGroup)
                        {
                            // create group for account
                            // create vehicle group with vehicle                            
                            groupName = string.Format("VehicleGroup_{0}_{1}", request.OrganizationId.ToString(), vehicle.Id.ToString());
                            if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                            group = new Group.Group(Group.GroupType.Single, Group.ObjectType.VehicleGroup, null,
                                                          Group.FunctionEnum.None, vehicle.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                            group = await _groupManager.Create(group);
                            vehicleGroupId = group.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var result = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                        else
                        {
                            vehicleGroupId = vehicle.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var result = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                    }
                }
                response.Code = Responcecode.Success;
                response.Message = "Created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ServiceResponse
                {
                    Message = "Exception :-" + ex.Message + ex.StackTrace,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<ServiceResponse> UpdateAccountAccessRelationship(AccountAccessRelationship request, ServerCallContext context)
        {
            bool result = true;
            var response = new ServiceResponse();
            try
            {

                // delete access relatioship for account or account group
                int accountGroupId = request.Id;
                string groupName;
                Group.Group group;
                if (!request.IsGroup)
                {
                    // create vehicle group with vehicle                    
                    groupName = string.Format("AccountGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                    group = new Group.Group(Group.GroupType.Single, Group.ObjectType.AccountGroup, null,
                                                    Group.FunctionEnum.None, request.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                    group = await _groupManager.Create(group);
                    accountGroupId = group.Id;
                }
                // delete access relatioship for vehicle or vehicle group
                if (request.OrganizationId > 0 && request.Id > 0)
                {
                    result = await _accountmanager.DeleteVehicleAccessRelationship(request.OrganizationId, accountGroupId, false);
                }
                if (result)
                {
                    foreach (var vehicle in request.VehiclesVehicleGroups)
                    {
                        int vehicleGroupId;
                        // create group type single
                        if (!vehicle.IsGroup)
                        {
                            // create group for account
                            // create vehicle group with vehicle                            
                            groupName = string.Format("VehicleGroup_{0}_{1}", request.OrganizationId.ToString(), vehicle.Id.ToString());
                            if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                            group = new Group.Group(Group.GroupType.Single, Group.ObjectType.VehicleGroup, null,
                                                          Group.FunctionEnum.None, vehicle.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                            group = await _groupManager.Create(group);
                            vehicleGroupId = group.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var accessRelationshipResult = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                        else
                        {
                            vehicleGroupId = vehicle.Id;
                            var accessRelationship = new account.entity.AccessRelationship((AccountComponent.ENUM.AccessRelationType)Convert.ToChar(request.AccessType), accountGroupId, vehicleGroupId);
                            var accessRelationshipResult = await _accountmanager.CreateAccessRelationship(accessRelationship);
                        }
                    }
                }
                response.Code = Responcecode.Success;
                response.Message = "Created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ServiceResponse
                {
                    Message = "Exception :-" + ex.Message + ex.StackTrace,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<ServiceResponse> DeleteAccountAccessRelationship(DeleteAccessRelationRequest request, ServerCallContext context)
        {
            bool result = true;
            var response = new ServiceResponse();
            try
            {

                // delete access relatioship for account or account group
                int accountGroupId = request.Id;
                if (!request.IsGroup)
                {
                    // create vehicle group with vehicle                    
                    string groupName = string.Format("AccountGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (groupName.Length > 50) groupName = groupName.Substring(0, 49);
                    Group.Group group = new Group.Group(Group.GroupType.Single, Group.ObjectType.AccountGroup, null,
                                        Group.FunctionEnum.None, request.Id, groupName, groupName, _mapper.TimeStamp(), request.OrganizationId);
                    group = await _groupManager.Create(group);
                    accountGroupId = group.Id;
                }
                // delete access relatioship for vehicle or vehicle group
                if (request.OrganizationId > 0 && request.Id > 0)
                {
                    result = await _accountmanager.DeleteVehicleAccessRelationship(request.OrganizationId, accountGroupId, false);
                }
                if (result) response.Code = Responcecode.Success;
                else response.Code = Responcecode.Failed;

                response.Message = "Deleted";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ServiceResponse
                {
                    Message = "Exception :-" + ex.Message + ex.StackTrace,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<AccessRelationshipResponse> GetAccessRelationship(AccessRelationshipFilter request, ServerCallContext context)
        {
            AccessRelationshipResponse accessRelationship = new AccessRelationshipResponse();

            try
            {
                if (request.OrganizationId > 0)
                {
                    AccountVehicleAccessRelationshipFilter filter = new AccountVehicleAccessRelationshipFilter();
                    filter.OrganizationId = request.OrganizationId;
                    var vehicleAccessRelation = await _accountmanager.GetAccountVehicleAccessRelationship(filter, true);
                    var accountAccessRelation = await _accountmanager.GetAccountVehicleAccessRelationship(filter, false);
                    accessRelationship.VehicleAccessRelationship.AddRange(_mapper.ToVehicleAccessRelationShip(vehicleAccessRelation));
                    accessRelationship.AccountAccessRelationship.AddRange(_mapper.ToVehicleAccessRelationShip(accountAccessRelation));
                    _logger.Info("Get AccessRelationshipAccount." + request.OrganizationId.ToString());
                }
                accessRelationship.Code = Responcecode.Success;
                return accessRelationship;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccessRelationshipResponse
                {
                    Message = "Exception :-" + ex.Message + ex.StackTrace,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<AccountVehiclesResponse> GetAccountsVehicles(AccessRelationshipFilter request, ServerCallContext context)
        {
            string validationMessage = string.Empty;
            AccountVehiclesResponse accountVehiclesResponse = new AccountVehiclesResponse();

            try
            {
                if (request.OrganizationId > 0)
                {
                    AccountVehicleAccessRelationshipFilter filter = new AccountVehicleAccessRelationshipFilter();
                    List<vehicleEntity.AccountVehicleEntity> vehicleList = new List<vehicleEntity.AccountVehicleEntity>();
                    List<AccountVehicleEntity> accountList = new List<AccountVehicleEntity>();
                    filter.OrganizationId = request.OrganizationId;
                    if (request.IsAccount)
                    {
                        accountList = await _accountmanager.GetAccount(filter, true);
                        vehicleList = await _vehicelManager.GetORGRelationshipVehicleGroupVehicles(request.OrganizationId, false);
                    }
                    else
                    {
                        accountList = await _accountmanager.GetAccount(filter, false);
                        vehicleList = await _vehicelManager.GetORGRelationshipVehicleGroupVehicles(request.OrganizationId, true);
                    }
                    List<AccountVehicleEntity> objVehiclelist = vehicleList.Select(a => new AccountVehicleEntity { Id = a.Id, Name = a.Name, Is_group = a.Is_group, Count = a.Count, RegistrationNo = a.RegistrationNo, VIN = a.VIN }).ToList();
                    accountVehiclesResponse.VehiclesVehicleGroup.AddRange(_mapper.ToAccountVehicles(objVehiclelist));
                    accountVehiclesResponse.AccountsAccountGroups.AddRange(_mapper.ToAccountVehicles(accountList));
                    _logger.Info("Get AccessRelationshipAccount." + request.OrganizationId.ToString());
                }
                accountVehiclesResponse.Code = Responcecode.Success;
                return accountVehiclesResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountVehiclesResponse
                {
                    Message = "Exception :-" + ex.Message + ex.StackTrace,
                    Code = Responcecode.Failed
                });
            }
        }


        #endregion

        #region AccountPreference

        public override async Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToPreference(request);
                preference.Exists = false;
                preference = await _preferencemanager.Create(preference);
                if (preference.Id.HasValue) request.Id = preference.Id.Value;
                if (preference.IconId > 0) { request.IconId = preference.IconId; }
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Created";
                response.AccountPreference = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Failed due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override async Task<AccountPreferenceResponse> UpdatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToPreference(request);
                preference.Exists = false;
                preference = await _preferencemanager.Update(preference);
                if (preference.Id.HasValue) request.Id = preference.Id.Value;
                if (preference.IconId > 0) { request.IconId = preference.IconId; }
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Updated";
                response.AccountPreference = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Failed due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override async Task<AccountPreferenceResponse> DeletePreference(IdRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _preferencemanager.Delete(request.Id, Preference.PreferenceType.Account);
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                if (result)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Preference Delete.";

                }
                else
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Preference Not Found.";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Failed due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override async Task<AccountPreferenceResponse> GetPreference(AccountPreferenceFilter request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = request.Id;
                //preferenceFilter.Ref_Id = request.RefId;
                preferenceFilter.PreferenceType = Preference.PreferenceType.Account; // (Preference.PreferenceType)Enum.Parse(typeof(Preference.PreferenceType), request.Preference.ToString());
                _logger.Info("Get account preference.");
                var result = await _preferencemanager.Get(preferenceFilter);
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Get";
                foreach (Preference.AccountPreference entity in result)
                {
                    response.AccountPreference = _mapper.ToPreferenceEntity(entity);
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Get Failed due to - " + ex.Message

                });
            }
        }
        #endregion

        #region Account Group
        public override async Task<AccountGroupResponce> CreateGroup(AccountGroupRequest request, ServerCallContext context)
        {
            try
            {
                AccountGroupResponce response = new AccountGroupResponce();
                response.AccountGroup = new AccountGroupRequest();
                Group.Group group = new Group.Group();
                group = _mapper.ToGroup(request);
                group = await _groupManager.Create(group);
                // check for exists
                response.AccountGroup.Exists = false;
                if (group.Exists)
                {
                    response.AccountGroup.Exists = true;
                    response.Message = "Duplicate Group";
                    response.Code = Responcecode.Conflict;
                    return response;
                }
                // Add group reference.                               
                if (group.Id > 0 && request.GroupRef != null && group.GroupType == Group.GroupType.Group)
                {
                    group.GroupRef = new List<Group.GroupRef>();
                    foreach (var item in request.GroupRef)
                    {
                        if (item.RefId > 0)
                            group.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId, Group_Id = group.Id });
                    }
                    bool accountRef = await _groupManager.AddRefToGroups(group.GroupRef);
                }
                request.Id = group.Id;
                request.CreatedAt = group.CreatedAt.Value;
                _logger.Info("Group Created:" + Convert.ToString(group.Name));
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Account group created.",
                    Code = Responcecode.Success,
                    AccountGroup = request
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<AccountGroupResponce> UpdateGroup(AccountGroupRequest request, ServerCallContext context)
        {
            try
            {
                Group.Group entity = new Group.Group();
                entity = _mapper.ToGroup(request);
                entity = await _groupManager.Update(entity);
                if (entity.Id > 0 && entity != null)
                {
                    if (request.GroupRef != null && Convert.ToInt16(request.GroupRef.Count) > 0)
                    {
                        entity.GroupRef = new List<Group.GroupRef>();
                        foreach (var item in request.GroupRef)
                        {
                            if (item.RefId > 0)
                                entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId, Group_Id = entity.Id });
                        }
                        if ((entity.GroupRef != null) && Convert.ToInt16(entity.GroupRef.Count) > 0)
                        {
                            bool accountRef = await _groupManager.UpdateRef(entity);
                        }
                        else
                        {
                            // delete existing reference
                            await _groupManager.RemoveRef(entity.Id);
                        }
                    }
                    else
                    {
                        // delete existing reference
                        await _groupManager.RemoveRef(entity.Id);
                    }
                }
                _logger.Info("Update Account Group :" + Convert.ToString(entity.Name));
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Account group updated for id: " + entity.Id,
                    Code = Responcecode.Success,
                    AccountGroup = request
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Account Group Update Failed :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<AccountGroupResponce> RemoveGroup(IdRequest request, ServerCallContext context)
        {
            try
            {
                bool result = await _groupManager.Delete(request.Id, Group.ObjectType.AccountGroup);
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Account Group deleted.",
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<AccountGroupRefResponce> AddAccountToGroups(AccountGroupRefRequest request, ServerCallContext context)
        {
            try
            {
                Group.Group group = new Group.Group();
                group.GroupRef = new List<Group.GroupRef>();
                AccountGroupRefResponce response = new AccountGroupRefResponce();
                bool result = false;

                if (request.GroupRef != null)
                {
                    foreach (var item in request.GroupRef)
                    {
                        group.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId, Group_Id = item.GroupId });
                    }
                    // add account to groups
                    if (group.GroupRef != null && Convert.ToInt16(group.GroupRef.Count) > 0)
                    {
                        result = _groupManager.AddRefToGroups(group.GroupRef).Result;
                    }
                }
                if (result)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Account Added to Account Group.";
                }
                else
                {
                    response.Code = Responcecode.Failed;
                    response.Message = "Account Addition to Group is Failed.";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountGroupRefResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        // Delete account from group
        public override async Task<AccountGroupResponce> DeleteAccountFromGroups(IdRequest request, ServerCallContext context)
        {
            try
            {

                AccountGroupResponce response = new AccountGroupResponce();
                bool result = false;
                result = await _groupManager.RemoveRefByRefId(request.Id);
                if (result)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Account Deleted from Group.";
                }
                else
                {
                    response.Code = Responcecode.Failed;
                    response.Message = "Account Deletion from Group is Failed.";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public async override Task<AccountGroupDataList> GetAccountGroup(AccountGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                AccountGroupDataList accountGroupList = new AccountGroupDataList();
                Group.GroupFilter ObjGroupFilter = new Group.GroupFilter();
                ObjGroupFilter.Id = request.Id;
                // filter based on account id
                ObjGroupFilter.RefId = request.AccountId;
                ObjGroupFilter.OrganizationId = request.OrganizationId;
                ObjGroupFilter.GroupRef = request.GroupRef;
                ObjGroupFilter.GroupRefCount = request.GroupRefCount;

                ObjGroupFilter.FunctionEnum = Group.FunctionEnum.None;
                ObjGroupFilter.ObjectType = Group.ObjectType.AccountGroup;
                ObjGroupFilter.GroupType = Group.GroupType.None;

                IEnumerable<Group.Group> ObjRetrieveGroupList = await _groupManager.Get(ObjGroupFilter);
                _logger.Info("Get account group.");
                foreach (var item in ObjRetrieveGroupList)
                {
                    AccountGroupRequest ObjResponce = new AccountGroupRequest();
                    ObjResponce = _mapper.ToAccountGroup(item);
                    accountGroupList.AccountGroupRequest.Add(ObjResponce);
                }
                accountGroupList.Message = "Account Group Details";
                accountGroupList.Code = Responcecode.Success;
                return await Task.FromResult(accountGroupList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AccountGroupDataList
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        // Get group details
        public async override Task<AccountGroupDetailsDataList> GetAccountGroupDetail(AccountGroupDetailsRequest request, ServerCallContext context)
        {
            try
            {
                Group.GroupFilter groupFilter = new Group.GroupFilter();
                AccountGroupDetailsDataList response = new AccountGroupDetailsDataList();
                AccountGroupDetail accountDetail = null;

                groupFilter.OrganizationId = request.OrganizationId;
                groupFilter.Id = request.AccountGroupId;
                groupFilter.GroupType = Group.GroupType.None;
                groupFilter.FunctionEnum = Group.FunctionEnum.None;
                groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                groupFilter.GroupRefCount = true;
                // all account group of organization with account count
                IEnumerable<Group.Group> accountGroups = await _groupManager.Get(groupFilter);
                // get access relationship 
                AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();

                foreach (Group.Group group in accountGroups)
                {
                    List<GroupRef> associatedVehicle = new List<GroupRef>();
                    accountDetail = new AccountGroupDetail();
                    accountDetail.GroupId = group.Id;
                    accountDetail.AccountGroupName = group.Name;
                    accountDetail.AccountCount = group.GroupRefCount;
                    accountDetail.OrganizationId = group.OrganizationId;
                    accountDetail.Type = Convert.ToString((char)group.GroupType);
                    accountDetail.CreatedAt = group.CreatedAt ?? 0;
                    accessFilter.AccountGroupId = group.Id;

                    var accessList = await _accountmanager.GetAccessRelationship(accessFilter);
                    int[] groupIds;
                    // Get vehicles count
                    if (accessList.Count > 0)
                    {
                        groupIds = accessList.Select(c => c.VehicleGroupId).ToArray();
                        var count = await _groupManager.GetVehiclesCount(groupIds, request.OrganizationId);
                        accountDetail.VehicleCount = count;
                    }
                    response.AccountGroupDetail.Add(accountDetail);
                }

                response.Message = "Get AccountGroup";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new AccountGroupDetailsDataList
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        #endregion

        #region Account Role

        public async override Task<AccountRoleResponse> AddRoles(AccountRoleRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.AccountRole role = new AccountComponent.entity.AccountRole();
                AccountRoleResponse response = new AccountRoleResponse();
                if (request != null && request.AccountRoles != null)
                {
                    role.OrganizationId = request.OrganizationId;
                    role.AccountId = request.AccountId;
                    role.RoleIds = new List<int>();
                    foreach (AccountRole accountRole in request.AccountRoles)
                    {
                        role.RoleIds.Add(accountRole.RoleId);
                    }
                    role.StartDate = DateTime.UtcNow;
                    role.EndDate = null;
                    var result = _accountmanager.AddRole(role).Result;
                }
                response.Message = "Roles added to account";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new AccountRoleResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public async override Task<AccountRoleResponse> RemoveRoles(AccountRoleDeleteRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                AccountRoleResponse response = new AccountRoleResponse();
                if (request != null)
                {
                    accountRole.OrganizationId = request.OrganizationId;
                    accountRole.AccountId = request.AccountId;
                }
                var result = await _accountmanager.RemoveRole(accountRole);
                response.Message = "Deleted Account roles.";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new AccountRoleResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public async override Task<AccountRoles> GetRoles(AccountRoleDeleteRequest request, ServerCallContext context)
        {

            try
            {
                AccountRoles response = new AccountRoles();
                AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                accountRole.AccountId = request.AccountId;
                accountRole.OrganizationId = request.OrganizationId;

                if (request != null && request.AccountId > 0 && request.OrganizationId > 0)
                {
                    accountRole.OrganizationId = request.OrganizationId;
                    accountRole.AccountId = request.AccountId;
                    var roles = await _accountmanager.GetRoles(accountRole);
                    _logger.Info("Get Roles");
                    foreach (AccountComponent.entity.KeyValue role in roles)
                    {
                        //response.Roles = new NameIdResponse();
                        response.Roles.Add(new NameIdResponse() { Id = role.Id, Name = role.Name });
                    }
                    response.Message = "Get Roles.";
                    response.Code = Responcecode.Success;
                    return await Task.FromResult(response);
                }
                else
                {
                    // validation message
                    response.Message = "Please provide accountid and organizationid to get roles details.";
                    response.Code = Responcecode.Failed;
                    return await Task.FromResult(response);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new AccountRoles
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        #endregion

        #region Private Helper Methods

        private MenuFeatureList MapMenuFeatureDtoToList(List<MenuFeatureDto> dtos)
        {
            var menuFeatures = new AccountMenuModel();
            var menuFeatureList = new MenuFeatureList();

            foreach (var dto in dtos)
            {
                if (dto.MenuId.HasValue)
                {
                    if (string.IsNullOrEmpty(dto.ParentMenuName))
                    {
                        menuFeatures.Menus.Add(new MainMenu()
                        {
                            FeatureId = dto.FeatureId,
                            MenuId = dto.MenuId.Value,
                            Name = dto.MenuName,
                            TranslatedMenuName = dto.TranslatedMenuName,
                            Key = dto.MenuKey,
                            Url = dto.MenuUrl,
                            SubMenus = new RepeatedField<SubMenu>()
                        });
                    }
                    else
                    {
                        var menuItem = menuFeatures.Menus.Where(m => m.Name.Equals(dto.ParentMenuName)).FirstOrDefault();

                        menuItem.SubMenus.Add(new SubMenu()
                        {
                            FeatureId = dto.FeatureId,
                            MenuId = dto.MenuId.Value,
                            Name = dto.MenuName,
                            TranslatedMenuName = dto.TranslatedMenuName,
                            Key = dto.MenuKey,
                            Url = dto.MenuUrl
                        });
                    }
                }

                menuFeatureList.Features.Add(new FeatureList()
                {
                    FeatureId = dto.FeatureId,
                    Name = dto.FeatureName,
                    Type = string.IsNullOrEmpty(dto.FeatureType) ? string.Empty : dto.FeatureType,
                    Key = string.IsNullOrEmpty(dto.FeatureKey) ? string.Empty : dto.FeatureKey,
                    Level = dto.FeatureLevel
                });
            }

            foreach (var menu in menuFeatures.Menus)
            {
                var subMenus = new RepeatedField<SubMenuList>();
                foreach (var subMenu in menu.SubMenus)
                {
                    subMenus.Add(new SubMenuList()
                    {
                        MenuId = subMenu.MenuId,
                        FeatureId = subMenu.FeatureId,
                        Name = subMenu.Name,
                        TranslatedName = string.IsNullOrEmpty(subMenu.TranslatedMenuName) ? subMenu.Name : string.Empty,
                        Key = string.IsNullOrEmpty(subMenu.Key) ? string.Empty : subMenu.Key,
                        Url = string.IsNullOrEmpty(subMenu.Url) ? string.Empty : subMenu.Url
                    });
                }
                var mainMenu = new MainMenuList();
                mainMenu.FeatureId = menu.FeatureId;
                mainMenu.MenuId = menu.MenuId;
                mainMenu.Name = menu.Name;
                mainMenu.TranslatedName = string.IsNullOrEmpty(menu.TranslatedMenuName) ? menu.Name : string.Empty;
                mainMenu.Key = string.IsNullOrEmpty(menu.Key) ? string.Empty : menu.Key;
                mainMenu.Url = string.IsNullOrEmpty(menu.Url) ? string.Empty : menu.Url;
                mainMenu.SubMenus.AddRange(subMenus);

                menuFeatureList.Menus.Add(mainMenu);
            }
            return menuFeatureList;
        }

        #endregion

        #region Single Sign On

        public async override Task<SSOToken> GenerateSSO(TokenSSORequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.TokenSSORequest ssoRequest = new AccountComponent.entity.TokenSSORequest();
                ssoRequest.AccountID = request.AccountID;
                ssoRequest.RoleID = request.RoleID;
                ssoRequest.OrganizaitonID = request.OrganizationID;
                ssoRequest.Email = request.Email;

                SSOToken responseDetails = new SSOToken();
                var response = await _accountIdentityManager.GenerateSSOToken(ssoRequest);
                if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseDetails.Token = response.Token;
                    responseDetails.Code = Responcecode.Success;
                    responseDetails.Message = response.Message;

                }
                else if (response?.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    responseDetails.Code = Responcecode.NotFound;
                    responseDetails.Message = response.Message;
                }
                else
                {
                    responseDetails.Code = Responcecode.NotFound;
                    responseDetails.Message = response.Message;
                }
                return await Task.FromResult(responseDetails);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new SSOToken
                {
                    Code = Responcecode.Failed,
                    Message = ex.StackTrace
                });
            }
        }

        #endregion

        #region Country
        public override async Task<ResponseCountry> GetCountryDetail(RequestCountry request, ServerCallContext context)
        {
            try
            {

                ResponseCountry response = new ResponseCountry();

                account.entity.CountryFilter countryRequest = new account.entity.CountryFilter
                {
                    Code = request.Code,
                    RegionType = request.RegionType,
                    DialCode = request.DialCode

                };

                var result = await _accountmanager.GetCountryDetails(countryRequest);

                var resCountry = JsonConvert.SerializeObject(result);
                response.Country.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<Country>>(resCountry,
                     new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));


                response.Code = Responcecode.Success;
                response.Message = "Country details fetched successfully.";

                return await Task.FromResult(response);
            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ResponseCountry
                {
                    Code = Responcecode.Failed,
                    Message = "Country details Failed due to - " + ex.Message
                });
            }
        }
        #endregion
    }
}
