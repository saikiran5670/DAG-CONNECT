using System;
using System.Collections.Generic;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.accountservicerest
{
    public class EntityMapper
    {
        public AccountComponent.entity.Account ToAccountEntity(AccountRequest request)
        {
            var account = new AccountComponent.entity.Account();
            account.Id = request.Id;
            account.EmailId = request.EmailId;
            account.Salutation = request.Salutation;
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.Password = request.Password;
            account.Organization_Id = request.Organization_Id;
            return account;
        }
        public AccountDetailsResponse ToAccountDetail(AccountComponent.entity.Account account)
        {
            AccountDetailsResponse response = new AccountDetailsResponse();

            response.Id = account.Id;
            response.EmailId = account.EmailId;
            response.Salutation = account.Salutation;
            response.FirstName = account.FirstName;
            response.LastName = account.LastName;
            response.OrganizationId = account.Organization_Id;
            return response;
        }
        public accountpreference.AccountPreference ToAccountPreference(AccountPreferenceRequest request)
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
            preference.DateFormatTypeId = request.DateFormatTypeId;
            preference.DriverId = request.DriverId;
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }
        public Group.Group ToAccountGroup(AccountGroupRequest request)
        {
            Group.Group group = new Group.Group();
            group.GroupRef=null;
            group.Id= request.Id;
            group.Name = request.Name;
            group.Description = request.Description;
            group.Argument = "";     
            group.OrganizationId = request.OrganizationId;
            if(request.Accounts != null)
            {
                group.GroupRef = new List<Group.GroupRef>();
                foreach (var groupref in request.Accounts)
                {
                    group.GroupRef.Add(new Group.GroupRef() { Group_Id= groupref.AccountGroupId, Ref_Id = groupref.AccountId});
                }
            }
            return group;
        }
    }
}
