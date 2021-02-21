using System;
using System.Collections.Generic;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;
namespace net.atos.daf.ct2.accountservice
{
    public class Mapper
    {
        public AccountRequest ToAccount(AccountComponent.entity.Account account)
        {
            AccountRequest request = new AccountRequest();

            request.Id = account.Id;
            request.EmailId = account.EmailId;
            request.Salutation = account.Salutation;
            request.FirstName = account.FirstName;
            request.LastName = account.LastName;           
            request.OrganizationId = account.Organization_Id;
            return request;
        }
        public AccountComponent.entity.Account ToAccountEntity(AccountRequest request)
        {
            var account = new AccountComponent.entity.Account();
            account.Id = request.Id;
            account.EmailId = request.EmailId;
            account.Salutation = request.Salutation;
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.Password = request.Password;
            account.Organization_Id = request.OrganizationId;
            account.StartDate = DateTime.Now;
            account.EndDate = null;
            return account;
        }
        public AccountRequest ToAccountDetail(AccountComponent.entity.Account account)
        {
            AccountRequest response = new AccountRequest();

            response.Id = account.Id;
            response.EmailId = account.EmailId;
            response.Salutation = account.Salutation;
            response.FirstName = account.FirstName;
            response.LastName = account.LastName;
            response.OrganizationId = account.Organization_Id;
            return response;
        }
        public accountpreference.AccountPreference ToPreference(AccountPreference request)
        {
            accountpreference.AccountPreference preference = new accountpreference.AccountPreference();
            preference.Id = request.Id;
            preference.RefId = request.RefId;
            preference.PreferenceType = Preference.PreferenceType.Account;
            preference.LanguageId = request.LanguageId;
            preference.TimezoneId = request.TimezoneId;
            preference.CurrencyId = request.CurrencyId;
            preference.UnitId = request.UnitId;
            preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatTypeId = request.DateFormatId;
            preference.DriverId = request.DriverId;
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }
        public AccountPreference ToPreferenceEntity(Preference.AccountPreference entity)
        {
            AccountPreference request = new AccountPreference();
            request.Id = entity.Id.HasValue ? entity.Id.Value : 0;
            request.RefId = entity.RefId;
            request.LanguageId = entity.LanguageId;
            request.TimezoneId = entity.TimezoneId;
            request.CurrencyId = entity.CurrencyId;
            request.UnitId = entity.UnitId;
            request.VehicleDisplayId = entity.VehicleDisplayId;
            request.DateFormatId = entity.DateFormatTypeId;
            request.DriverId = entity.DriverId;
            request.TimeFormatId = entity.TimeFormatId;
            request.LandingPageDisplayId = entity.LandingPageDisplayId;
            return request;
        }
        // group mapping
        public Group.Group ToGroup(AccountGroupRequest request)
        {
            Group.Group entity = new Group.Group();
                entity.Id = request.Id;
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Argument = "";//request.Argument;                
                entity.FunctionEnum = group.FunctionEnum.None;
                entity.GroupType = group.GroupType.Group;
                entity.ObjectType = group.ObjectType.AccountGroup;
                entity.OrganizationId = request.OrganizationId;
                entity.GroupRef = new List<Group.GroupRef>();
                // foreach (var item in request.GroupRef)
                // {
                //     if (item.RefId > 0)
                //         entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId });
                // }
                return entity;
        }
        public AccessRelationship ToAccessRelationShip(AccountComponent.entity.AccessRelationship entity)
        {
            AccessRelationship request = new AccessRelationship();

            request.Id = entity.Id;
            request.AccessRelationType = entity.AccessRelationType.ToString();
            request.AccountGroupId = entity.AccountGroupId;
            request.VehicleGroupId = entity.VehicleGroupId;
            return request;
        }
        public AccountGroupRequest ToAccountGroup(Group.Group entity)
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

        // private AccountType SetEnumAccountType(AccountComponent.ENUM.AccountType type)
        // {
        //     AccountType accountType = AccountType.None;

        //     if (type == AccountComponent.ENUM.AccountType.None)
        //     {
        //         accountType = AccountType.None;
        //     }
        //     else if (type == AccountComponent.ENUM.AccountType.SystemAccount)
        //     {
        //         accountType = AccountType.SystemAccount;
        //     }
        //     else if (type == AccountComponent.ENUM.AccountType.PortalAccount)
        //     {
        //         accountType = AccountType.PortalAccount;
        //     }
        //     return accountType;
        // }
        // private AccountComponent.ENUM.AccountType GetEnum(int value)
        // {
        //     AccountComponent.ENUM.AccountType accountType;
        //     switch (value)
        //     {
        //         case 0:
        //             accountType = AccountComponent.ENUM.AccountType.None;
        //             break;
        //         case 1:
        //             accountType = AccountComponent.ENUM.AccountType.SystemAccount;
        //             break;
        //         case 2:
        //             accountType = AccountComponent.ENUM.AccountType.PortalAccount;
        //             break;
        //         default:
        //             accountType = AccountComponent.ENUM.AccountType.PortalAccount;
        //             break;
        //     }
        //     return accountType;
        // }
    }
}
