namespace net.atos.daf.ct2.organization.entity
{
    public class PreferenceResponse
    {
        public int PreferenceId { get; set; }
        public int OrganizatioId { get; set; }
        public int LanguageName { get; set; }
        public int Timezone { get; set; }
        public int Currency { get; set; }
        public int Unit { get; set; }
        public int VehicleDisplay { get; set; }
        public int DateFormatType { get; set; }
        public int TimeFormat { get; set; }
        public string LandingPageDisplay { get; set; }
    }
    public class OrganizationDetailsResponse
    {
        public int id { get; set; }
        public int preferenceId { get; set; }
        public string org_id { get; set; }
        public string name { get; set; }
        public string street { get; set; }
        public string street_number { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
        public string vehicle_default_opt_in { get; set; }
        public string driver_default_opt_in { get; set; }
        public string LanguageName { get; set; }
        public string Timezone { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string DateFormatType { get; set; }
        public string TimeFormat { get; set; }
    }
}
