using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;


namespace net.atos.daf.ct2.accountservice
{
    public class AccountManagementService : AccountService.AccountServiceBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly AccountComponent.IAccountManager accountmanager;
        private readonly Preference.IPreferenceManager preferencemanager;
        private readonly Group.IGroupManager groupmanager;
        //private readonly IAuditTraillib auditlog;
        
        public AccountManagementService(ILogger<GreeterService> logger, AccountComponent.IAccountManager _accountmanager, Preference.IPreferenceManager _preferencemanager, Group.IGroupManager _groupmanager)
        {
            _logger = logger;
            accountmanager = _accountmanager;
            preferencemanager = _preferencemanager;
            groupmanager = _groupmanager;
            //auditlog =  _auditlog;
        }

        public override Task<AccountData> Create(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                //account.Dob = null;//request.Dob.Seconds;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount; //GetEnum((int)request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Create(account).Result;
                //auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Account Service","Create Account",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.SUCCESS,"Create Account" + account.EmailId,1,2,null);
                // response 
                AccountData response = new AccountData();

                if (account.isDuplicate)
                {
                    response.Message = "The duplicate account, please provide unique email address.";
                    response.Code = Responcecode.Failed;
                }
                else
                {
                    response.Code = Responcecode.Success;
                    request.Id = account.Id;
                    response.Message = "Created";
                    response.Account = request;
                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountData
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Faile due to - " + ex.Message,
                    Account = null
                });
            }
        }
        public override Task<AccountData> Update(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                //account.Dob = request.Dob.Seconds;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount; ; //GetEnum((int)request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Update(account).Result;

                // response 
                AccountData response = new AccountData();
                response.Code = Responcecode.Success;
                response.Message = "Updated";
                response.Account = request;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountData
                {
                    Code = Responcecode.Failed,
                    Message = "Account Updation Faile due to - " + ex.Message,
                    Account = null
                });
            }
        }
        public override Task<AccountResponse> Delete(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                //account.Dob = request.Dob.Seconds;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount; //GetEnum((int)request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = accountmanager.Delete(account).Result;

                // response 
                AccountResponse response = new AccountResponse();
                response.Code = Responcecode.Success;
                response.Message = "Delete";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Deletion Faile due to - " + ex.Message
                });
            }
        }
        public override Task<AccountResponse> ChangePassword(AccountRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.Account account = new AccountComponent.entity.Account();
                account.Id = request.Id;
                account.EmailId = request.EmailId;
                account.Salutation = request.Salutation;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                account.Password = request.Password;
                //account.Dob = request.Dob.Seconds;
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;//GetEnum((int)request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = accountmanager.ChangePassword(account).Result;

                // response 
                AccountResponse response = new AccountResponse();
                response.Code = Responcecode.Success;
                response.Message = "Change Password";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Change Password faile due to with reason : " + ex.Message
                });
            }
        }

        public override Task<AccountDataList> Get(AccountFilter request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.AccountFilter filter = new AccountComponent.entity.AccountFilter();
                filter.Id = request.Id;
                filter.OrganizationId = request.OrganizationId;
                filter.Name = request.Name;
                filter.Email = request.Email;
                filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount; //;GetEnum((int)request.AccountType);
                filter.AccountIds = null;
                if (request.AccountIds != null && Convert.ToString(request.AccountIds).Length > 0)
                {
                    filter.AccountIds = request.AccountIds;
                }
                var result = accountmanager.Get(filter).Result;
                // response 
                AccountDataList response = new AccountDataList();
                foreach (AccountComponent.entity.Account entity in result)
                {
                    response.Accounts.Add(MapToRequest(entity));
                }
                response.Code = Responcecode.Success;
                response.Message = "Get";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountDataList
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<AccountDetailsResponse> GetAccountDetail(AccountGroupDetailsRequest request, ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.AccountFilter filter = new AccountComponent.entity.AccountFilter();

                List<AccountComponent.entity.Account> accounts = new List<AccountComponent.entity.Account>();
                List<int> accountIds = new List<int>();
                AccountDetailsResponse response = new AccountDetailsResponse();
                AccountDetails accountDetails = new AccountDetails();
                List<string> accountGroupName = null;
                if (request.GroupId > 0)
                {
                    // AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();
                    // accessFilter.AccountGroupId = filter.GroupId;
                    // var accessRelationship = accountmanager.GetAccessRelationship(accessFilter).Result;
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
                    accountIds = accountmanager.GetRoleAccounts(request.RoleId).Result;
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
                    filter.OrganizationId = request.OrganizationId;
                    filter.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    filter.AccountIds = null;
                    // list of account for organization 
                    accounts = accountmanager.Get(filter).Result.ToList();
                }

                // account group details                 
                foreach (AccountComponent.entity.Account entity in accounts)
                {
                    accountDetails.Account = MapToRequest(entity);
                    Group.GroupFilter groupFilter = new Group.GroupFilter();
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.RefId = request.AccountId;
                    groupFilter.ObjectType = Group.ObjectType.None;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    groupFilter.GroupType = Group.GroupType.None;

                    var vehicleGroupList = groupmanager.Get(groupFilter).Result;
                    if (vehicleGroupList != null)
                    {
                        accountGroupName = new List<string>();
                        foreach (Group.Group vGroup in vehicleGroupList)
                        {
                            accountGroupName.Add(vGroup.Name);
                        }
                    }
                    if (accountGroupName != null)
                    {
                        accountDetails.AccountGroups = string.Join(",", accountGroupName);
                    }

                    // Get roles   
                    AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                    accountRole.AccountId = request.AccountId;
                    accountRole.OrganizationId = request.OrganizationId;
                    var roles = accountmanager.GetRoles(accountRole).Result;
                    if (roles != null && Convert.ToInt32(roles.Count) > 0)
                    {
                        accountDetails.Roles = string.Join(",", roles);
                    }
                    // End Get Roles
                    response.AccountDetails.Add(accountDetails);
                }
                response.Code = Responcecode.Success;
                response.Message = "Get";
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AccountDetailsResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }
        }
        // End Account

        // Begin AccessRelationship
        public override Task<AccessRelationshipResponse> CreateAccessRelationship(AccessRelationship request, ServerCallContext context)
        {
            string validationMessage = string.Empty;
            try
            {
                // access relation ship entity
                AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
                // response 
                AccessRelationshipResponse response = new AccessRelationshipResponse();

                accessRelationship.Id = request.Id;
                accessRelationship.AccountGroupId = request.AccountGroupId;
                accessRelationship.VehicleGroupId = request.VehicleGroupId;
                if (!string.IsNullOrWhiteSpace(request.AccessRelationType) && request.AccessRelationType == "R")
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
                }
                if (!string.IsNullOrWhiteSpace(request.AccessRelationType) && request.AccessRelationType == "W")
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadWrite;
                }
                else
                {
                    validationMessage = "The AccessType should be ReadOnly / ReadWrite.(R/W).";
                }
                //account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount; //GetEnum((int)request.Type);                
                accessRelationship.StartDate = DateTime.Now;
                accessRelationship.EndDate = null;
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    accessRelationship = accountmanager.CreateAccessRelationship(accessRelationship).Result;
                    response.AccessRelationship.Id = accessRelationship.Id;
                    response.Code = Responcecode.Success;
                    response.Message = "AccessRelationship Created";
                }
                else
                {
                    response.Message = validationMessage;
                    response.Code = Responcecode.Success;

                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccessRelationshipResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Faile due to - " + ex.Message

                });
            }
        }
        public override Task<AccessRelationshipResponse> UpdateAccessRelationship(AccessRelationship request, ServerCallContext context)
        {
            string validationMessage = string.Empty;
            try
            {
                // access relation ship entity
                AccountComponent.entity.AccessRelationship accessRelationship = new AccountComponent.entity.AccessRelationship();
                // response 
                AccessRelationshipResponse response = new AccessRelationshipResponse();

                accessRelationship.Id = request.Id;
                accessRelationship.AccountGroupId = request.AccountGroupId;
                accessRelationship.VehicleGroupId = request.VehicleGroupId;
                if (!string.IsNullOrWhiteSpace(request.AccessRelationType) && request.AccessRelationType == "R")
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadOnly;
                }
                if (!string.IsNullOrWhiteSpace(request.AccessRelationType) && request.AccessRelationType == "W")
                {
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ReadWrite;
                }
                else
                {
                    validationMessage = "The AccessType should be ReadOnly / ReadWrite.(R/W).";
                }
                //account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount; //GetEnum((int)request.Type);                
                accessRelationship.StartDate = DateTime.Now;
                accessRelationship.EndDate = null;
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    accessRelationship = accountmanager.UpdateAccessRelationship(accessRelationship).Result;
                    response.Code = Responcecode.Success;
                    response.Message = "AccessRelationship Updated";
                }
                else
                {
                    response.Message = validationMessage;
                    response.Code = Responcecode.Success;

                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccessRelationshipResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Faile due to - " + ex.Message

                });
            }
        }
        public override Task<AccessRelationshipResponse> DeleteAccessRelationship(AccessRelationship request, ServerCallContext context)
        {
            string validationMessage = string.Empty;
            try
            {
                // response 
                AccessRelationshipResponse response = new AccessRelationshipResponse();
                if (request == null || request.Id <= 0)
                {
                    validationMessage = "The Access Id need to be provide for deleting access relationship.";
                }
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    var result = accountmanager.DeleteAccessRelationship(request.Id).Result;
                    response.Code = Responcecode.Success;
                    response.Message = "AccessRelationship Deleted";
                }
                else
                {
                    response.Message = validationMessage;
                    response.Code = Responcecode.Success;

                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccessRelationshipResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Faile due to - " + ex.Message

                });
            }
        }
        public override Task<AccessRelationshipDataList> GetAccessRelationship(AccessRelationshipFilter request, ServerCallContext context)
        {
            string validationMessage = string.Empty;
            try
            {
                // access relation ship entity
                AccountComponent.entity.AccessRelationshipFilter filter = new AccountComponent.entity.AccessRelationshipFilter();
                // response 
                AccessRelationshipDataList response = new AccessRelationshipDataList();

                filter.AccountId = request.AccountId;
                filter.AccountGroupId = request.AccountGroupId;
                if (request.AccountId == 0 && request.AccountGroupId == 0)
                {
                    validationMessage = "Please provide AccountId or AccountGroupId to get AccessRelationship.";
                }
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    var accessResult = accountmanager.GetAccessRelationship(filter).Result;
                    foreach (AccountComponent.entity.AccessRelationship accessRelationship in accessResult)
                    {
                        response.AccessRelationship.Add(MapToAccessRelationShipRequest(accessRelationship));
                    }
                    response.Code = Responcecode.Success;
                    response.Message = "AccessRelationship Get";
                }
                else
                {
                    response.Message = validationMessage;
                    response.Code = Responcecode.Success;

                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccessRelationshipDataList
                {
                    Code = Responcecode.Failed,
                    Message = "Account Creation Faile due to - " + ex.Message

                });
            }
        }
        // Begin AccountPreference

        public override Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference.Id = request.Id;
                preference.RefId = request.RefId;
                preference.PreferenceType = Preference.PreferenceType.Account; //(Preference.PreferenceType)Enum.Parse(typeof(Preference.PreferenceType), request.PreferenceType.ToString());
                preference.LanguageId = request.LanguageId;
                preference.TimezoneId = request.TimezoneId;
                preference.CurrencyId = request.CurrencyId;
                preference.UnitId = request.UnitId;
                preference.VehicleDisplayId = request.VehicleDisplayId;
                preference.DateFormatTypeId = request.DateFormatId;
                preference.DriverId = request.DriverId;                
                preference.TimeFormatId = request.TimeFormatId;
                preference.LandingPageDisplayId = request.LandingPageDisplayId;
                preference = preferencemanager.Create(preference).Result;
                if (preference.Id.HasValue) request.Id = preference.Id.Value;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Created";
                response.AccountPreference = request;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Faile due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }

        public override Task<AccountPreferenceResponse> UpdatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference.Id = request.Id;
                preference.RefId = request.RefId;
                preference.PreferenceType = Preference.PreferenceType.Account; //(Preference.PreferenceType)Enum.Parse(typeof(Preference.PreferenceType), request.PreferenceType.ToString());
                preference.LanguageId = request.LanguageId;
                preference.TimezoneId = request.TimezoneId;
                preference.CurrencyId = request.CurrencyId;
                preference.UnitId = request.UnitId;
                preference.VehicleDisplayId = request.VehicleDisplayId;
                preference.DateFormatTypeId = request.DateFormatId;
                preference.DriverId = request.DriverId;               
                preference.TimeFormatId = request.TimeFormatId;
                preference.LandingPageDisplayId = request.LandingPageDisplayId;
                preference = preferencemanager.Update(preference).Result;
                if (preference.Id.HasValue) request.Id = preference.Id.Value;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Updated";
                response.AccountPreference = request;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Faile due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override Task<AccountPreferenceResponse> DeletePreference(AccountPreferenceFilter request, ServerCallContext context)
        {
            try
            {
                // Preference.AccountPreference preference = new Preference.AccountPreference();
                // preference.Id = request.Id;
                // preference.RefId = request.RefId;
                var result = preferencemanager.Delete(request.RefId).Result;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Delete.";                
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Faile due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override Task<AccountPreferenceDataList> GetPreference(AccountPreferenceFilter request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = request.Id;
                preferenceFilter.Ref_Id = request.RefId;
                preferenceFilter.PreferenceType = Preference.PreferenceType.Account; // (Preference.PreferenceType)Enum.Parse(typeof(Preference.PreferenceType), request.Preference.ToString());

                var result = preferencemanager.Get(preferenceFilter).Result;
                // response 
                AccountPreferenceDataList response = new AccountPreferenceDataList();
                response.Code = Responcecode.Success;
                response.Message = "Get";
                foreach (Preference.AccountPreference entity in result)
                {
                    response.Preference.Add(MapToPreferenceRequest(entity));
                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AccountPreferenceDataList
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Get Faile due to - " + ex.Message

                });
            }
        }
        // End Account Preference

        // Begin Account Group
        public override Task<AccountGroupResponce> CreateGroup(AccountGroupRequest request, ServerCallContext context)
        {
            try
            {
                Group.Group entity = new Group.Group();
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Argument = "";
                //entity.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                entity.FunctionEnum = group.FunctionEnum.None;
                entity.GroupType = group.GroupType.Group;
                entity.ObjectType = group.ObjectType.AccountGroup;
                entity.OrganizationId = request.OrganizationId;

                entity.GroupRef = new List<Group.GroupRef>();
                if (request.GroupRef != null)
                {
                    foreach (var item in request.GroupRef)
                    {
                        if (item.RefId > 0)
                            entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId });
                    }
                }
                var result = groupmanager.Create(entity).Result;
                if (result.Id > 0 && entity.GroupRef != null)
                {
                    if (entity.GroupRef.Count > 0)
                    {
                        bool AddvehicleGroupRef = groupmanager.UpdateRef(entity).Result;
                    }
                }
                _logger.LogInformation("Group Created:" + Convert.ToString(entity.Name));
                return Task.FromResult(new AccountGroupResponce
                {
                    Id = entity.Id,
                    Message = "Account group created with id:- " + entity.Id,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in create account group :CreateGroup with exception - " + ex.Message);
                return Task.FromResult(new AccountGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public override Task<AccountGroupResponce> UpdateGroup(AccountGroupRequest request, ServerCallContext context)
        {
            try
            {
                Group.Group entity = new Group.Group();
                entity.Id = request.Id;
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Argument = "";//request.Argument;
                // entity.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                entity.FunctionEnum = group.FunctionEnum.None;
                entity.GroupType = group.GroupType.Group;
                entity.ObjectType = group.ObjectType.AccountGroup;
                entity.OrganizationId = request.OrganizationId;

                entity.GroupRef = new List<Group.GroupRef>();
                foreach (var item in request.GroupRef)
                {
                    if (item.RefId > 0)
                        entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId });
                }
                var result = groupmanager.Update(entity).Result;

                if (result.Id > 0 && entity != null)
                {
                    if (entity.GroupRef.Count > 0)
                    {
                        bool AddvehicleGroupRef = groupmanager.UpdateRef(entity).Result;
                    }
                }
                _logger.LogInformation("Update Account Group :" + Convert.ToString(entity.Name));
                return Task.FromResult(new AccountGroupResponce
                {
                    Id = entity.Id,
                    Message = "Account group updated for id: " + entity.Id,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in create account group :CreateGroup with exception - " + ex.Message);
                return Task.FromResult(new AccountGroupResponce
                {
                    Message = "Account Group Update Failed :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override Task<AccountGroupResponce> DeleteGroup(IdRequest request, ServerCallContext context)
        {
            try
            {
                bool result = groupmanager.Delete(request.Id).Result;

                _logger.LogInformation("Delete group method in account group.");

                return Task.FromResult(new AccountGroupResponce
                {
                    Id = request.Id,
                    Message = "Account Group deleted with id:- " + request,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service delete group method.");

                return Task.FromResult(new AccountGroupResponce
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
                ObjGroupFilter.OrganizationId = request.OrganizationId;
                //ObjGroupFilter.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                ObjGroupFilter.FunctionEnum = Group.FunctionEnum.None;
                ObjGroupFilter.GroupRef = request.GroupRef;
                ObjGroupFilter.GroupRefCount = request.GroupRefCount;
                //ObjGroupFilter.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                ObjGroupFilter.ObjectType = Group.ObjectType.AccountGroup;
                //ObjGroupFilter.GroupType = (group.GroupType)Enum.Parse(typeof(group.GroupType), request.GroupType.ToString());
                ObjGroupFilter.GroupType = Group.GroupType.Group;

                IEnumerable<Group.Group> ObjRetrieveGroupList = groupmanager.Get(ObjGroupFilter).Result;
                foreach (var item in ObjRetrieveGroupList)
                {
                    AccountGroupRequest ObjResponce = new AccountGroupRequest();
                    ObjResponce = MapToAccountGroupResponse(item);
                    accountGroupList.AccountGroupRequest.Add(ObjResponce);
                }
                accountGroupList.Message = "Vehicles data retrieved";
                accountGroupList.Code = Responcecode.Success;
                return await Task.FromResult(accountGroupList);
            }
            catch (Exception ex)
            {
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
                groupFilter.GroupType = Group.GroupType.Group;
                groupFilter.FunctionEnum = Group.FunctionEnum.None;
                groupFilter.ObjectType = Group.ObjectType.AccountGroup;
                groupFilter.GroupRefCount = true;
                // all account group of organization with account count
                IEnumerable<Group.Group> accountGroups = groupmanager.Get(groupFilter).Result;
                // get access relationship 
                AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();
                foreach (Group.Group group in accountGroups)
                {
                    accountDetail = new AccountGroupDetail();
                    accountDetail.GroupId = group.Id;
                    accountDetail.AccountGroupName = group.Name;
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
                    response.AccountGroupDetail.Add(accountDetail);
                }
                response.Message = "Get AccountGroup";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AccountGroupDetailsDataList
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        // End Account Group

        // Begin Account Role

        public async override Task<AccountRoleResponse> AddRoles(AccountRoleRequest request, ServerCallContext context)
        {

            try
            {
                List<AccountComponent.entity.AccountRole> accountRoles = new List<AccountComponent.entity.AccountRole>();
                AccountRoleResponse response = new AccountRoleResponse();
                if (request != null && request.AccountRoles != null)
                {
                    foreach (AccountRole accountRole in request.AccountRoles)
                    {
                        AccountComponent.entity.AccountRole role = new AccountComponent.entity.AccountRole();
                        role.OrganizationId = accountRole.OrganizationId;
                        role.AccountId = accountRole.AccountId;
                        role.RoleId = accountRole.RoleId;
                        accountRoles.Add(role);
                    }
                }
                var result = accountmanager.AddRole(accountRoles).Result;
                response.Message = "Account Added to Roles";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AccountRoleResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public async override Task<AccountRoleResponse> RemoveRoles(AccountRole request, ServerCallContext context)
        {

            try
            {
                AccountComponent.entity.AccountRole accountRole = new AccountComponent.entity.AccountRole();
                AccountRoleResponse response = new AccountRoleResponse();
                if (request != null)
                {
                    accountRole.OrganizationId = request.OrganizationId;
                    accountRole.AccountId = request.AccountId;
                    accountRole.RoleId = request.RoleId;

                }
                var result = accountmanager.RemoveRole(accountRole).Result;
                response.Message = "Deleted Account from roles";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AccountRoleResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        // End Account Role

        // Begin Private Methods
        private AccountRequest MapToRequest(AccountComponent.entity.Account account)
        {
            AccountRequest request = new AccountRequest();

            request.Id = account.Id;
            request.EmailId = account.EmailId;
            request.Salutation = account.Salutation;
            request.FirstName = account.FirstName;
            request.LastName = account.LastName;
            //request.Dob = new Google.Protobuf.WellKnownTypes.Timestamp();
            //if (account.Dob.HasValue) request.Dob.Seconds = account.Dob.Value;
            //request.Dob.Seconds  = account.Dob.HasValue ? account.Dob.Value : 0;
            //request.Type = SetEnumAccountType(account.AccountType);
            //request.Type = (AccountType)Enum.Parse(typeof(AccountType), account.AccountType.ToString());
            request.OrganizationId = account.Organization_Id;
            return request;
        }
        private AccountPreference MapToPreferenceRequest(Preference.AccountPreference entity)
        {
            AccountPreference request = new AccountPreference();
            request.Id = entity.Id.HasValue ?  entity.Id.Value : 0;
            request.RefId = entity.RefId ;            
            request.LanguageId = entity.LanguageId;
            request.TimezoneId = entity.TimezoneId;
            request.CurrencyId = entity.CurrencyId;
            request.UnitId = entity.UnitId ;
            request.VehicleDisplayId = entity.VehicleDisplayId;
            request.DateFormatId = entity.DateFormatTypeId;            
            request.DriverId = entity.DriverId;
            request.TimeFormatId = entity.TimeFormatId;
            request.LandingPageDisplayId = entity.LandingPageDisplayId ;
            return request;
        }
        // Map to access relationship from entity to request
        private AccessRelationship MapToAccessRelationShipRequest(AccountComponent.entity.AccessRelationship entity)
        {
            AccessRelationship request = new AccessRelationship();

            request.Id = entity.Id;
            request.AccessRelationType = entity.AccessRelationType.ToString();
            request.AccountGroupId = entity.AccountGroupId;
            request.VehicleGroupId = entity.VehicleGroupId;
            return request;
        }
        private AccountGroupRequest MapToAccountGroupResponse(Group.Group entity)
        {
            AccountGroupRequest request = new AccountGroupRequest();
            request.Id = entity.Id;
            request.Name = entity.Name;
            request.Description = entity.Description;
            //request.Argument = entity.Argument;

            //request.FunctionEnum = (FunctionEnum)Enum.Parse(typeof(FunctionEnum), entity.FunctionEnum.ToString());
            //request.FunctionEnum = entity.FunctionEnum.ToString();
            //request.GroupType = (GroupType)Enum.Parse(typeof(GroupType), entity.GroupType.ToString());
            //request.GroupType = entity.GroupType.ToString();
            //request.ObjectType = (ObjectType)Enum.Parse(typeof(ObjectType), entity.ObjectType.ToString());
            //request.ObjectType = entity.ObjectType.ToString();
            request.OrganizationId = entity.OrganizationId;
            request.GroupRefCount = entity.GroupRefCount;
            if (entity.GroupRef != null)
            {
                foreach (var item in entity.GroupRef)
                {
                    request.GroupRef.Add(new AccountGroupRef() { RefId = item.Ref_Id, GroupId = item.Group_Id });
                }
            }

            return request;
        }
        private AccountType SetEnumAccountType(AccountComponent.ENUM.AccountType type)
        {
            AccountType accountType = AccountType.None;

            if (type == AccountComponent.ENUM.AccountType.None)
            {
                accountType = AccountType.None;
            }
            else if (type == AccountComponent.ENUM.AccountType.SystemAccount)
            {
                accountType = AccountType.SystemAccount;
            }
            else if (type == AccountComponent.ENUM.AccountType.PortalAccount)
            {
                accountType = AccountType.PortalAccount;
            }
            return accountType;
        }
        private AccountComponent.ENUM.AccountType GetEnum(int value)
        {
            AccountComponent.ENUM.AccountType accountType;
            switch (value)
            {
                case 0:
                    accountType = AccountComponent.ENUM.AccountType.None;
                    break;
                case 1:
                    accountType = AccountComponent.ENUM.AccountType.SystemAccount;
                    break;
                case 2:
                    accountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    break;
                default:
                    accountType = AccountComponent.ENUM.AccountType.PortalAccount;
                    break;
            }
            return accountType;
        }
        // End Private Methods
    }
}
