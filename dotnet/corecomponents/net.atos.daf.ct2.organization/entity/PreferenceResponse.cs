namespace net.atos.daf.ct2.organization.entity
{
    public class PreferenceResponse
    {
        public int PreferenceId { get; set; }
        public int OrganizationId { get; set; }
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
        public int Id { get; set; }
        public int PreferenceId { get; set; }
        public string OrgId { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string VehicleDefaultOptIn { get; set; }
        public string DriverDefaultOptIn { get; set; }
        public string LanguageName { get; set; }
        public string Timezone { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string DateFormatType { get; set; }
        public string TimeFormat { get; set; }
    }
}
