using Preference = net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.organizationservicerest.entity
{
    public class EntityMapper
    {
       public accountpreference.AccountPreference ToOrganizationPreference(AccountPreferenceRequest request)
        {
            accountpreference.AccountPreference preference = new accountpreference.AccountPreference();
            preference.Id = request.Id;
            preference.RefId = request.OrgId;
            preference.PreferenceType = Preference.PreferenceType.Organization;
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
    }
}
