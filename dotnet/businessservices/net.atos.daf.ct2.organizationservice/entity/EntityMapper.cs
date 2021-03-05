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
            preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatTypeId = request.DateFormatId;
            // preference.DriverId = request.DriverId;
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }
        
    }
}
