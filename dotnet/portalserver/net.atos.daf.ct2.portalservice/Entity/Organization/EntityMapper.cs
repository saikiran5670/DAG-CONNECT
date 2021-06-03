using AccountBusinessService = net.atos.daf.ct2.accountservice;
namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class EntityMapper
    {
        public AccountBusinessService.AccountPreference ToOrganizationPreference(AccountBusinessService.AccountPreference request)
        {
            AccountBusinessService.AccountPreference preference = new AccountBusinessService.AccountPreference();
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

        // public AccountBusinessService.AccountOrganizationResponse ToPreferenceResponse(accountservice.AccountPreference request)
        // {
        //     OrganizationBusinessService.OrganizationPreference objResponse=new OrganizationBusinessService.OrganizationPreference();
        //     objResponse.Id = request.PreferenceId;
        //     objResponse.OrgId = request.OrganizatioId;   
        //     if (!(string.IsNullOrEmpty(request.Currency)))
        //     {
        //          objResponse.Currency= request.Currency;
        //     }
        //     if (!(string.IsNullOrEmpty(request.LanguageName)))
        //     {
        //          objResponse.Language = request.LanguageName;
        //     }
        //      if (!(string.IsNullOrEmpty(request.TimeFormat)))
        //     {
        //         objResponse.TimeFormat = request.TimeFormat;
        //     }
        //      if (!(string.IsNullOrEmpty(request.DateFormatType)))
        //     {
        //           objResponse.DateFormat = request.DateFormatType;
        //     }           
        //     if (!(string.IsNullOrEmpty(request.VehicleDisplay)))
        //     {
        //           objResponse.VehicleDisplay = request.VehicleDisplay;
        //     }
        //       if (!(string.IsNullOrEmpty(request.LandingPageDisplay)))
        //     {
        //            objResponse.LandingPageDisplay = request.LandingPageDisplay;
        //     }
        //     if (!(string.IsNullOrEmpty(request.Unit)))
        //     {
        //           objResponse.Unit = request.Unit;
        //     }
        //     if (!(string.IsNullOrEmpty(request.Timezone)))
        //     {
        //            objResponse.Timezone = request.Timezone;         
        //     }
        //     return objResponse;
        // }

        // public net.atos.daf.ct2.organizationservice.OrgGetResponse ToOrganizationResponse( net.atos.daf.ct2.organization.entity.OrganizationResponse request)
        // {
        //    // OrganizationResponse objResponse=new OrganizationResponse();
        //    net.atos.daf.ct2.organizationservice.OrgGetResponse objResponse=new organizationservice.OrgGetResponse();
        //     objResponse.Id = request.Id;
        //     objResponse.Type=request.type;
        //     objResponse.Name=request.name;
        //     objResponse.AddressStreet=request.street;
        //     objResponse.AddressType=request.address_type;
        //     objResponse.AddressStreetNumber=request.street_number;
        //     objResponse.PostalCode=request.postal_code;
        //     objResponse.City=request.city;
        //     objResponse.CountryCode=request.country_code;
        //     objResponse.OrganizationId=request.org_id;
        //     objResponse.Referenced=request.reference_date; 
        //     objResponse.VehicleOptIn=request.vehicle_default_opt_in;
        //     objResponse.DriverOptIn=request.driver_default_opt_in;   
        //      return objResponse;          
        // }    

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
    }
}
