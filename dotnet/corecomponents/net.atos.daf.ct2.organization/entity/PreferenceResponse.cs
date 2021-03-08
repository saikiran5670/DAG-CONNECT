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
       // public string Driver { get; set; }
    }
}
