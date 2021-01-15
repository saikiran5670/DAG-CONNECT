using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;



namespace net.atos.daf.ct2.accountservice
{
    public class AccountManagementService : AccountService.AccountServiceBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly AccountComponent.IAccountManager accountmanager;
        private readonly Preference.IPreferenceManager preferencemanager;
        private readonly Group.GroupManager groupmanager;
        public AccountManagementService(ILogger<GreeterService> logger, AccountComponent.IAccountManager _accountmanager,Preference.IPreferenceManager _preferencemanager, Group.GroupManager _groupmanager)
        {
            _logger = logger;
            accountmanager = _accountmanager;
            preferencemanager = _preferencemanager;
            groupmanager = _groupmanager;
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
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Create(account).Result;

                // response 
                AccountData response = new AccountData();
                response.Code  = Responcecode.Success;
                response.Message = "Created";
                response.Account = request;

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
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                account = accountmanager.Update(account).Result;

                // response 
                AccountData response = new AccountData();
                response.Code  = Responcecode.Success;
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
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = accountmanager.Delete(account).Result;

                // response 
                AccountResponse response = new AccountResponse();
                response.Code  = Responcecode.Success;
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
                account.Dob = request.Dob.Seconds;
                account.AccountType = GetEnum((int) request.Type);
                account.Organization_Id = request.OrganizationId;
                account.StartDate = DateTime.Now;
                account.EndDate = null;
                var result = accountmanager.Delete(account).Result;

                // response 
                AccountResponse response = new AccountResponse();
                response.Code  = Responcecode.Success;
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

         public override Task<AccountDataList> Get(AccountFilter request , ServerCallContext context)
        {
            try
            {
                AccountComponent.entity.AccountFilter filter = new AccountComponent.entity.AccountFilter();
                filter.Id = request.Id;
                filter.OrganizationId = request.OrganizationId;                
                filter.AccountType = GetEnum((int) request.AccountType);                
                filter.AccountIds = null;
                if(request.AccountIds != null && Convert.ToString(request.AccountIds).Length >0)
                {
                    filter.AccountIds = request.AccountIds;                    
                }                
                var result = accountmanager.Get(filter).Result;
                // response 
                AccountDataList response = new AccountDataList();
                foreach(AccountComponent.entity.Account entity in result)
                {
                    response.Accounts.Add(MapToRequest(entity));
                }                
                response.Code  = Responcecode.Success;
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
        // Begin AccountPreference

        public override Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference.Id = request.Id;
                preference.Ref_Id = request.RefId;
                preference.PreferenceType = (Preference.PreferenceType)  Enum.Parse(typeof(Preference.PreferenceType), request.PreferenceType.ToString());
                preference.Language_Id = request.LanguageId;
                preference.Timezone_Id = request.TimezoneId;
                preference.Currency_Type = (Preference.CurrencyType)  Enum.Parse(typeof(Preference.CurrencyType), request.CurrencyType.ToString());
                preference.Unit_Type = (Preference.UnitType)  Enum.Parse(typeof(Preference.UnitType), request.UnitType.ToString());
                preference.VehicleDisplay_Type = (Preference.VehicleDisplayType)  Enum.Parse(typeof(Preference.VehicleDisplayType), request.VehicleDisplayType.ToString());
                preference.DateFormat_Type = (Preference.DateFormatDisplayType)  Enum.Parse(typeof(Preference.DateFormatDisplayType), request.DateFormatType.ToString());
                preference = preferencemanager.Create(preference).Result;

                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code  = Responcecode.Success;
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
                preference.Ref_Id = request.RefId;
                preference.PreferenceType = (Preference.PreferenceType)  Enum.Parse(typeof(Preference.PreferenceType), request.PreferenceType.ToString());
                preference.Language_Id = request.LanguageId;
                preference.Timezone_Id = request.TimezoneId;
                preference.Currency_Type = (Preference.CurrencyType)  Enum.Parse(typeof(Preference.CurrencyType), request.CurrencyType.ToString());
                preference.Unit_Type = (Preference.UnitType)  Enum.Parse(typeof(Preference.UnitType), request.UnitType.ToString());
                preference.VehicleDisplay_Type = (Preference.VehicleDisplayType)  Enum.Parse(typeof(Preference.VehicleDisplayType), request.VehicleDisplayType.ToString());
                preference.DateFormat_Type = (Preference.DateFormatDisplayType)  Enum.Parse(typeof(Preference.DateFormatDisplayType), request.DateFormatType.ToString());
                preference = preferencemanager.Update(preference).Result;

                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code  = Responcecode.Success;
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
        public override Task<AccountPreferenceResponse> DeletePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference.Id = request.Id;
                preference.Ref_Id = request.RefId;               
                var result = preferencemanager.Delete(request.Id).Result;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code  = Responcecode.Success;
                response.Message = "Preference Delete";
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
        public override Task<AccountPreferenceDataList> GetPreference(AccountPreferenceFilter request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = request.Id;
                preferenceFilter.Ref_Id = request.RefId; 
                preferenceFilter.PreferenceType = (Preference.PreferenceType)  Enum.Parse(typeof(Preference.PreferenceType), request.Preference.ToString());

                var result = preferencemanager.Get(preferenceFilter).Result;
                // response 
                AccountPreferenceDataList response = new AccountPreferenceDataList();
                response.Code  = Responcecode.Success;
                response.Message = "Get";
                foreach(Preference.AccountPreference entity in result)
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
                entity.Argument = request.Argument;
                entity.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                entity.GroupType = (group.GroupType)Enum.Parse(typeof(group.GroupType), request.GroupType.ToString());
                entity.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                entity.OrganizationId = request.OrganizationId;

                entity.GroupRef = new List<Group.GroupRef>();
                foreach (var item in request.GroupRef)
                {
                    entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId });
                }
                var result = groupmanager.Create(entity).Result;

                if (result.Id > 0)
                {
                    bool AddvehicleGroupRef = groupmanager.UpdateRef(entity).Result;
                }
                _logger.LogInformation("Created Account Group :" + Convert.ToString(entity.Name));                
                return Task.FromResult(new AccountGroupResponce
                {
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
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Argument = request.Argument;
                entity.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                entity.GroupType = (group.GroupType)Enum.Parse(typeof(group.GroupType), request.GroupType.ToString());
                entity.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                entity.OrganizationId = request.OrganizationId;

                entity.GroupRef = new List<Group.GroupRef>();
                foreach (var item in request.GroupRef)
                {
                    entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId });
                }
                var result = groupmanager.Create(entity).Result;

                if (result.Id > 0)
                {
                    bool AddvehicleGroupRef = groupmanager.UpdateRef(entity).Result;
                }
                _logger.LogInformation("Update Account Group :" + Convert.ToString(entity.Name));                
                return Task.FromResult(new AccountGroupResponce
                {
                    Message = "Account group updated with id: " + entity.Id,
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

        public override Task<AccountGroupResponce> DeleteGroup(DeleteRecordRequest request, ServerCallContext context)
        {
            try
            {
                bool result = groupmanager.Delete(request.Id).Result;
                
                _logger.LogInformation("Delete group method in account group.");
                
                return Task.FromResult(new AccountGroupResponce
                {
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

        public async override Task<AccountGroupResponce> GetGroupDetails(AccountGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                Group.GroupFilter ObjGroupFilter = new Group.GroupFilter();
                ObjGroupFilter.Id = request.Id;
                ObjGroupFilter.OrganizationId = request.OrganizationId;
                ObjGroupFilter.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                ObjGroupFilter.GroupRef = request.GroupRef;
                ObjGroupFilter.GroupRefCount = request.GroupRefCount;
                ObjGroupFilter.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                ObjGroupFilter.GroupType = (group.GroupType)Enum.Parse(typeof(group.GroupType), request.GroupType.ToString());

                IEnumerable<Group.Group> ObjRetrieveGroupList = groupmanager.Get(ObjGroupFilter).Result;
                // foreach (var item in ObjRetrieveVehicleList)
                // {
                //     VehicleRequest ObjResponce = new VehicleRequest();
                //     ObjResponce.Id = item.ID;
                //     ObjResponce.Organizationid = item.Organization_Id;
                //     ObjResponce.Name = item.Name;
                //     ObjResponce.Vin = item.VIN;
                //     ObjResponce.LicensePlateNumber = item.License_Plate_Number;
                //     ObjResponce.Status = (VehicleStatusType)(char)item.Status;

                //     ObjVehicleList.Vehicles.Add(ObjResponce);
                // }
                // ObjVehicleList.Message = "Vehicles data retrieved";
                // ObjVehicleList.Code = Responcecode.Success;
                // return await Task.FromResult(ObjVehicleList);

                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Exception " ,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AccountGroupResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        // End Account Group

        // Begin Private Methods
        private AccountRequest MapToRequest(AccountComponent.entity.Account account)
        {
            AccountRequest request = new AccountRequest();

            request.Id = account.Id;
            request.EmailId = account.EmailId ;
            request.Salutation = account.Salutation;
            request.FirstName = account.FirstName ;
            request.LastName = account.LastName;
            request.Dob = new Google.Protobuf.WellKnownTypes.Timestamp();
            if (account.Dob.HasValue) request.Dob.Seconds = account.Dob.Value;
            //request.Dob.Seconds  = account.Dob.HasValue ? account.Dob.Value : 0;
            //request.Type = SetEnumAccountType(account.AccountType);
            request.Type = (AccountType)  Enum.Parse(typeof(AccountType), account.AccountType.ToString());
            request.OrganizationId = account.Organization_Id;            
            return request;
        }
        private AccountPreference MapToPreferenceRequest(Preference.AccountPreference entity)
        {
            AccountPreference request = new AccountPreference();

            request.Id = entity.Id.Value;            
            request.RefId = request.RefId;
            request.PreferenceType = (PreferenceType)  Enum.Parse(typeof(PreferenceType), entity.PreferenceType.ToString());
            request.LanguageId = entity.Language_Id;
            request.TimezoneId = entity.Timezone_Id;
            request.CurrencyType = (CurrencyType)  Enum.Parse(typeof(CurrencyType), entity.Currency_Type.ToString());
            request.UnitType = (UnitType)  Enum.Parse(typeof(UnitType), entity.Unit_Type.ToString());
            request.VehicleDisplayType = (VehicleDisplayType)  Enum.Parse(typeof(VehicleDisplayType), entity.VehicleDisplay_Type.ToString());
            request.DateFormatType = (DateFormatDisplayType)  Enum.Parse(typeof(DateFormatDisplayType), entity.DateFormat_Type.ToString());              
            return request;
        }
        

        private AccountType SetEnumAccountType(AccountComponent.ENUM.AccountType type)
        {
            AccountType accountType = AccountType.None;

            if ( type == AccountComponent.ENUM.AccountType.None)
            {
                accountType = AccountType.None;
            }
            else if ( type == AccountComponent.ENUM.AccountType.SystemAccount)
            {
                accountType = AccountType.SystemAccount;
            }
            else if ( type == AccountComponent.ENUM.AccountType.PortalAccount)
            {
                accountType = AccountType.PortalAccount;
            }
            return accountType;
        }
        private AccountComponent.ENUM.AccountType GetEnum(int value)
        {
            AccountComponent.ENUM.AccountType accountType;
            switch(value)
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
