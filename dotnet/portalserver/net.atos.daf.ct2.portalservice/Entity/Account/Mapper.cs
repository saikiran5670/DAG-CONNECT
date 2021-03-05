using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.utilities;
using AccountBusinessService = net.atos.daf.ct2.accountservice;


namespace net.atos.daf.ct2.portalservice.Account
{
    public class Mapper
    {
        public AccountBusinessService.AccountRequest ToAccount(AccountRequest request)
        {
            var account = new AccountBusinessService.AccountRequest();
            account.Id = request.Id;
            account.EmailId = request.EmailId;
            if (!string.IsNullOrEmpty(request.Type)) account.Type = request.Type;
            account.Salutation = request.Salutation;
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.Password = request.Password;
            account.OrganizationId = request.OrganizationId;
            account.DriverId = request.DriverId;
            account.CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            account.StartDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            account.EndDate = 0;
            return account;
        }
        public  AccountResponse ToAccount(AccountBusinessService.AccountRequest response)
        {
            var account = new AccountResponse();
            if (response == null) return account;            
            account.Id = response.Id;
            account.EmailId = response.EmailId;
            if (!string.IsNullOrEmpty(response.Type)) account.Type = response.Type;
            account.Salutation = response.Salutation;
            account.FirstName = response.FirstName;
            account.LastName = response.LastName;
            account.Password = response.Password;
            account.OrganizationId = response.OrganizationId;
            account.DriverId = response.DriverId;
            account.PreferenceId = response.PreferenceId;
            account.BlobId = response.BlobId;
            account.CreatedAt = response.CreatedAt;
            return account;
        }        
        public List<AccountResponse> ToAccounts(AccountBusinessService.AccountDataList response)
        {
            var accounts = new List<AccountResponse>();
            if (response == null || response.Accounts == null) return accounts;
            foreach(var account in response.Accounts)
            {
                accounts.Add(ToAccount(account));
            }            
            return accounts;
        }        
        public AccountBusinessService.AccountFilter ToAccountFilter(AccountFilterRequest request)
        {
            AccountBusinessService.AccountFilter response = new AccountBusinessService.AccountFilter();

            response.Id = request.Id;
            response.Email = request.Email;
            response.Name = request.Name;
            response.OrganizationId = request.OrganizationId;
            response.AccountIds = request.AccountIds;
            response.AccountGroupId= request.AccountGroupId;
            return response;
        }
        public AccountBusinessService.AccountGroupDetailsRequest ToAccountDetailsFilter(AccountDetailRequest request)
        {
            AccountBusinessService.AccountGroupDetailsRequest response = new AccountBusinessService.AccountGroupDetailsRequest();

            response.AccountId = request.AccountId;
            response.OrganizationId = request.OrganizationId;
            response.AccountGroupId = request.AccountGroupId;
            response.VehicleGroupId = request.VehicleGroupId;
            response.RoleId = request.RoleId;
            response.Name = request.Name;
            return response;
        }
        public List<AccountDetailsResponse> ToAccountDetailsResponse(AccountBusinessService.AccountDetailsResponse request)
        {
            List<AccountDetailsResponse> response = new List<AccountDetailsResponse>();
            AccountDetailsResponse accountDetails = new AccountDetailsResponse();
            if (request == null) return response;
            foreach(var accountDetail in request.AccountDetails)
            {
                var account = accountDetail.Account;
                accountDetails = new AccountDetailsResponse();
                accountDetails.Id = account.Id;
                accountDetails.EmailId = account.EmailId;
                accountDetails.Salutation = account.Salutation;
                accountDetails.FirstName = account.FirstName;
                accountDetails.LastName = account.LastName;                
                accountDetails.OrganizationId = account.OrganizationId;
                // roles
                accountDetails.Roles = new List<KeyValue>();
                if (accountDetail.Roles!= null )
                {
                    foreach (var role in accountDetail.Roles)
                    {
                        accountDetails.Roles.Add(new KeyValue() { Id = role.Id, Name = role.Name });
                    }
                }
                // account groups
                accountDetails.AccountGroups = new List<KeyValue>();
                if (accountDetail.AccountGroups != null)
                {
                    foreach (var group in accountDetail.AccountGroups)
                    {
                        accountDetails.AccountGroups.Add(new KeyValue() { Id = group.Id, Name = group.Name });
                    }
                }
                response.Add(accountDetails);
            }
            return response;
        }
        public AccountBusinessService.AccountPreference ToAccountPreference(AccountPreferenceRequest request)
        {
            AccountBusinessService.AccountPreference preference = new AccountBusinessService.AccountPreference
            {
                Id = request.Id,
                RefId = request.RefId,
                PreferenceType = "A",
                LanguageId = request.LanguageId,
                TimezoneId = request.TimezoneId,
                CurrencyId = request.CurrencyId,
                UnitId = request.UnitId,
                VehicleDisplayId = request.VehicleDisplayId,
                DateFormatId = request.DateFormatTypeId,
                TimeFormatId = request.TimeFormatId,
                LandingPageDisplayId = request.LandingPageDisplayId
            };
            return preference;
        }
        public AccountPreferenceResponse ToAccountPreference(AccountBusinessService.AccountPreference request)
        {
            AccountPreferenceResponse preference = new AccountPreferenceResponse();
            preference.Id = request.Id;            
            preference.LanguageId = request.LanguageId;
            preference.TimezoneId = request.TimezoneId;
            preference.CurrencyId = request.CurrencyId;
            preference.UnitId = request.UnitId;
            preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatTypeId = request.DateFormatId;
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }

        public AccountBusinessService.AccountGroupRequest ToAccountGroup(AccountGroupRequest request)
        {
            AccountBusinessService.AccountGroupRequest group = new AccountBusinessService.AccountGroupRequest();

            group.Id = request.Id;
            group.Name = request.Name;
            group.Description = request.Description;            
            group.OrganizationId = request.OrganizationId;
            group.RefId = request.RefId;
            if (!string.IsNullOrEmpty(request.GroupType))
            {
                group.GroupType = request.GroupType;
            }
            if (request.Accounts != null)
            {
                //group.GroupRef = new List<AccountBusinessService.AccountGroupRef>();
                foreach (var groupref in request.Accounts)
                {
                    if(groupref.AccountId>0)
                    group.GroupRef.Add(new AccountBusinessService.AccountGroupRef() { GroupId = groupref.AccountGroupId, RefId= groupref.AccountId });
                }
            }
            return group;
        }
        public AccountGroupResponse ToAccountGroup(AccountBusinessService.AccountGroupResponce request)
        {
            AccountGroupResponse group = new AccountGroupResponse();
            if (request.AccountGroup == null) return group;

            group.Id = request.AccountGroup.Id;
            group.Name = request.AccountGroup.Name;
            group.Description = request.AccountGroup.Description;
            group.RefId = request.AccountGroup.RefId;
            group.OrganizationId = request.AccountGroup.OrganizationId;
            // group type
            if (!string.IsNullOrEmpty(request.AccountGroup.GroupType))
            {
                group.GroupType = request.AccountGroup.GroupType;
            }
            
            if (request.AccountGroup.GroupRef != null)
            {
                group.GroupRef = new List<GroupRef>();
                foreach (var groupref in request.AccountGroup.GroupRef)
                {
                    group.GroupRef.Add(new GroupRef() { AccountGroupId = groupref.GroupId ,  AccountId = groupref.RefId });
                }
            }
            return group;
        }
        public AccountBusinessService.AccountGroupDetailsRequest ToAccountGroupFilter(AccountGroupFilterRequest request )
        {
            AccountBusinessService.AccountGroupDetailsRequest filter = new AccountBusinessService.AccountGroupDetailsRequest();
            if (request == null) return filter;
            filter.AccountId = request.AccountId;
            filter.OrganizationId = request.OrganizationId;
            filter.AccountGroupId = request.AccountGroupId;
            return filter;
        }
            
        public AccountBusinessService.AccountRoleRequest ToRole(AccountRoleRequest request)
        {
            AccountBusinessService.AccountRoleRequest response = new AccountBusinessService.AccountRoleRequest();
            if (request == null) return response;
            response.AccountId = request.AccountId;
            response.OrganizationId = request.OrganizationId;
            if (request != null && request.Roles!=null && Convert.ToInt16(request.Roles.Count) > 0)
            {
                foreach (var role in request.Roles)
                {
                    response.AccountRoles.Add(new AccountBusinessService.AccountRole() { RoleId = role });
                }
            }
            return response;
        }

    }
}
