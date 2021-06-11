using net.atos.daf.ct2.organization.entity;
using static net.atos.daf.ct2.utilities.CommonEnums;
using Preference = net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.organizationservice.entity
{
    public class EntityMapper
    {
        public accountpreference.AccountPreference ToOrganizationPreference(AccountPreference request)
        {
            accountpreference.AccountPreference preference = new accountpreference.AccountPreference();
            preference.Id = request.Id;
            preference.RefId = request.RefId;
            preference.PreferenceType = Preference.PreferenceType.Account;
            preference.LanguageId = request.LanguageId;
            preference.TimezoneId = request.TimezoneId;
            preference.CurrencyId = request.CurrencyId;
            preference.UnitId = request.UnitId;
            //preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatTypeId = request.DateFormatId;
            // preference.DriverId = request.DriverId;
            preference.TimeFormatId = request.TimeFormatId;
            // preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }

        public OrganizationPreference ToPreferenceResponse(PreferenceResponse request)
        {
            OrganizationPreference objResponse = new OrganizationPreference();
            objResponse.Id = request.PreferenceId;
            objResponse.OrgId = request.OrganizationId;
            objResponse.Currency = request.Currency;
            objResponse.Language = request.LanguageName;
            objResponse.TimeFormat = request.TimeFormat;
            objResponse.DateFormat = request.DateFormatType;
            //objResponse.VehicleDisplay = request.VehicleDisplay;
            objResponse.Unit = request.Unit;
            objResponse.Timezone = request.Timezone;


            return objResponse;
        }

        public net.atos.daf.ct2.organizationservice.OrgGetResponse ToOrganizationResponse(net.atos.daf.ct2.organization.entity.OrganizationResponse request)
        {
            // OrganizationResponse objResponse=new OrganizationResponse();
            net.atos.daf.ct2.organizationservice.OrgGetResponse objResponse = new OrgGetResponse();
            objResponse.Id = request.Id;
            objResponse.Type = request.Type;
            objResponse.Name = request.Name;
            objResponse.AddressStreet = request.Street;
            objResponse.AddressType = request.AddressType;
            objResponse.AddressStreetNumber = request.StreetNumber;
            objResponse.PostalCode = request.PostalCode;
            objResponse.City = request.City;
            objResponse.CountryCode = request.CountryCode;
            objResponse.OrganizationId = request.OrgId;
            objResponse.Referenced = request.ReferenceDate;
            objResponse.VehicleOptIn = request.VehicleDefaultOptIn;
            objResponse.DriverOptIn = request.DriverDefaultOptIn;
            objResponse.IsActive = request.State == (char)State.Active ? true : false;
            return objResponse;
        }
        public net.atos.daf.ct2.organizationservice.AllOrganization ToListOfOrganizationResponse(net.atos.daf.ct2.organization.entity.Organization request)
        {
            net.atos.daf.ct2.organizationservice.AllOrganization organization = new net.atos.daf.ct2.organizationservice.AllOrganization();
            organization.OrganizationId = request.Id;
            organization.OrganizationName = request.Name;
            return organization;
        }

        public net.atos.daf.ct2.organizationservice.OrgCreateRequest TOOrgCreateResponse(net.atos.daf.ct2.organizationservice.OrgCreateRequest request)
        {

            net.atos.daf.ct2.organizationservice.OrgCreateRequest objResponse = new OrgCreateRequest();
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

            net.atos.daf.ct2.organizationservice.OrgUpdateRequest objResponse = new OrgUpdateRequest();
            objResponse.Id = request.Id;
            objResponse.VehicleDefaultOptIn = request.VehicleDefaultOptIn;
            objResponse.DriverDefaultOptIn = request.DriverDefaultOptIn;
            return objResponse;
        }
        public net.atos.daf.ct2.organizationservice.OrgDetailResponse ToOrganizationDetailsResponse(net.atos.daf.ct2.organization.entity.OrganizationDetailsResponse request)
        {
            net.atos.daf.ct2.organizationservice.OrgDetailResponse objResponse = new OrgDetailResponse();
            objResponse.Id = request.Id;
            objResponse.PreferenceId = request.PreferenceId;
            objResponse.OrganizationId = request.OrgId;
            objResponse.OrganizationName = request.Name;
            objResponse.AddressStreet = request.Street;
            objResponse.AddressStreetNumber = request.StreetNumber;
            objResponse.PostalCode = request.PostalCode;
            objResponse.City = request.City;
            objResponse.Country = request.CountryCode;
            objResponse.VehicleOptIn = request.VehicleDefaultOptIn;
            objResponse.DriverOptIn = request.DriverDefaultOptIn;
            objResponse.Currency = request.Currency;
            objResponse.Timezone = request.Timezone;
            objResponse.TimeFormat = request.TimeFormat;
            objResponse.DateFormat = request.DateFormatType;
            objResponse.LanguageName = request.LanguageName;
            objResponse.Unit = request.Unit;
            return objResponse;
        }

    }
}
