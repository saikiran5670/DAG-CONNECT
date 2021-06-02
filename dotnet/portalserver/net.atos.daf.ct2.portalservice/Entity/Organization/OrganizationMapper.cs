using AccountBusinessService = net.atos.daf.ct2.accountservice;
using OrganizationBusinessService = net.atos.daf.ct2.organizationservice;


namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class OrganizationMapper
    {
        public OrganizationBusinessService.OrgCreateRequest ToOragnizationRequest(OrganizationRequest request)
        {
            var orgRequest = new OrganizationBusinessService.OrgCreateRequest();
            orgRequest.Id = request.Id;
            orgRequest.OrgId = request.Org_id;
            orgRequest.Type = request.Type;
            orgRequest.Name = request.Name;
            orgRequest.AddressType = request.Address_type;
            orgRequest.Street = request.Street;
            orgRequest.StreetNumber = request.Street_number;
            orgRequest.City = request.City;
            orgRequest.CountryCode = request.Country_code;
            orgRequest.ReferenceDate = request.Reference_date.ToString();
            return orgRequest;
        }
        public OrganizationBusinessService.OrgUpdateRequest ToOragnizationUpdateRequest(OrganizationRequest request)
        {
            var orgRequest = new OrganizationBusinessService.OrgUpdateRequest();
            orgRequest.Id = request.Id;
            orgRequest.VehicleDefaultOptIn = request.Vehicle_default_opt_in;
            orgRequest.DriverDefaultOptIn = request.Driver_default_opt_in;

            return orgRequest;
        }
        public net.atos.daf.ct2.organizationservice.OrgCreateRequest TOOrgUpdateResponse(net.atos.daf.ct2.organizationservice.OrgCreateRequest request)
        {

            net.atos.daf.ct2.organizationservice.OrgCreateRequest objResponse = new organizationservice.OrgCreateRequest();
            objResponse.Id = request.Id;
            objResponse.Type = request.Type;
            objResponse.Name = request.Name;
            objResponse.Street = request.Street;
            objResponse.AddressType = request.AddressType;
            objResponse.StreetNumber = request.StreetNumber;
            objResponse.PostalCode = request.PostalCode;
            objResponse.City = request.City;
            objResponse.CountryCode = request.CountryCode;
            objResponse.OrgId = request.OrgId;
            objResponse.ReferenceDate = request.ReferenceDate;
            objResponse.VehicleDefaultOptIn = "I";
            objResponse.DriverDefaultOptIn = "I";
            return objResponse;
        }

        public net.atos.daf.ct2.organizationservice.OrgUpdateRequest TOOrgUpdateResponse(net.atos.daf.ct2.organizationservice.OrgUpdateRequest request)
        {

            net.atos.daf.ct2.organizationservice.OrgUpdateRequest objResponse = new organizationservice.OrgUpdateRequest();
            objResponse.Id = request.Id;
            objResponse.VehicleDefaultOptIn = request.VehicleDefaultOptIn;
            objResponse.DriverDefaultOptIn = request.DriverDefaultOptIn;
            return objResponse;
        }


        //  public AccountBusinessService.AccountPreference ToAccountPreference(AccountPreferenceRequest request)
        // {
        //     AccountBusinessService.AccountPreference preference = new AccountBusinessService.AccountPreference
        //     {
        //         Id = request.Id,
        //        // RefId = request.RefId,
        //         PreferenceType = "O",
        //         LanguageId = request.LanguageId,
        //         TimezoneId = request.TimezoneId,
        //         CurrencyId = request.CurrencyId,
        //         UnitId = request.UnitId,
        //         VehicleDisplayId = request.VehicleDisplayId,
        //         DateFormatId = request.DateFormatTypeId,
        //         TimeFormatId = request.TimeFormatId,
        //         LandingPageDisplayId = request.LandingPageDisplayId
        //     };
        //     return preference;
        // }
        public net.atos.daf.ct2.organizationservice.AccountPreference ToOrganizationPreference(AccountBusinessService.AccountPreference request)
        {
            net.atos.daf.ct2.organizationservice.AccountPreference preference = new OrganizationBusinessService.AccountPreference();
            // AccountBusinessService.AccountPreference preference = new AccountBusinessService.AccountPreference();
            preference.Id = request.Id;
            preference.RefId = request.RefId;
            preference.PreferenceType = request.PreferenceType;
            preference.LanguageId = request.LanguageId;
            preference.TimezoneId = request.TimezoneId;
            preference.CurrencyId = request.CurrencyId;
            preference.UnitId = request.UnitId;
            preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatId = request.DateFormatId;
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }
        public AccountBusinessService.AccountPreference ToAccountPreference(AccountBusinessService.AccountPreference request)
        {
            net.atos.daf.ct2.accountservice.AccountPreference preference = new AccountBusinessService.AccountPreference();
            // AccountBusinessService.AccountPreference preference = new AccountBusinessService.AccountPreference();
            preference.Id = request.Id;
            preference.LanguageId = request.LanguageId;
            preference.TimezoneId = request.TimezoneId;
            preference.CurrencyId = request.CurrencyId;
            preference.UnitId = request.UnitId;
            preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatId = request.DateFormatId;
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            preference.PreferenceType = request.PreferenceType;
            return preference;
        }
        public AccountBusinessService.AccountPreference ToAccountPreference(net.atos.daf.ct2.portalservice.Account.AccountPreferenceRequest request)
        {
            AccountBusinessService.AccountPreference preference = new AccountBusinessService.AccountPreference
            {
                Id = request.Id,
                RefId = request.RefId,
                PreferenceType = request.PreferenceType,
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
    }
}
