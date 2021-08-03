namespace net.atos.daf.ct2.accountpreference
{
    public class AccountPreference
    {
        public int? Id { get; set; }
        public int RefId { get; set; }
        public PreferenceType PreferenceType { get; set; }
        public int LanguageId { get; set; }
        public int TimezoneId { get; set; }
        public int CurrencyId { get; set; }
        public int UnitId { get; set; }
        public int VehicleDisplayId { get; set; }
        public int DateFormatTypeId { get; set; }
        public int TimeFormatId { get; set; }
        public int LandingPageDisplayId { get; set; }
        public bool Active { get; set; }
        public bool Exists { get; set; }
        public bool RefIdNotValid { get; set; }
        public int IconId { get; set; }
        public string IconByte { get; set; }
        public int CreatedBy { get; set; }
        public int PageRefreshTime { get; set; }

    }

}
