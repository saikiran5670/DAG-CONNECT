namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountPreferenceRequest
    {
        public int Id { get; set; }
        public int RefId { get; set; }
        public int LanguageId { get; set; }
        public int TimezoneId { get; set; }
        public int CurrencyId { get; set; }
        public int UnitId { get; set; }
        public int VehicleDisplayId { get; set; }
        public int DateFormatTypeId { get; set; }
        public int TimeFormatId { get; set; }
        public int LandingPageDisplayId { get; set; }
        public string PreferenceType { get; set; }
        public int IconId { get; set; }
        public string IconByte { get; set; }

    }
    public class AccountPreferenceResponse
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int TimezoneId { get; set; }
        public int CurrencyId { get; set; }
        public int UnitId { get; set; }
        public int VehicleDisplayId { get; set; }
        public int DateFormatTypeId { get; set; }
        public int TimeFormatId { get; set; }
        public int LandingPageDisplayId { get; set; }
        public int IconId { get; set; }
        public string IconByte { get; set; }
    }
}
