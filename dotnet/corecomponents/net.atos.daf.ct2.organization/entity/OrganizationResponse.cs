using System.Collections.Generic;

namespace net.atos.daf.ct2.organization.entity
{
    public class OrganizationResponse
    {
        public int Id { get; set; }
        public string org_id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string address_type { get; set; }
        public string street { get; set; }
        public string street_number { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
        public string reference_date { get; set; }
        public char state { get; set; }
        public int preference_id { get; set; }
        public string vehicle_default_opt_in { get; set; }
        public string driver_default_opt_in { get; set; }
        public List<OrganizationResponse> OrganizationList { get; set; }

    }
}
