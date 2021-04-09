using System;

namespace net.atos.daf.ct2.organization.entity
{
    public class PreferenceResponse
    {
        public int PreferenceId { get; set; }
        public int OrganizatioId { get; set; }        
        public string LanguageName { get; set; }
        public string Timezone { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string VehicleDisplay { get; set; }
        public string DateFormatType { get; set; }
        public string TimeFormat { get; set; }
        public string LandingPageDisplay { get; set; }      
    }
    public class OrganizationDetailsResponse
    {
        public int id { get; set; }
        public string org_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
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
