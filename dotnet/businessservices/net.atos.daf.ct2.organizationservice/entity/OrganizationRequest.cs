using System;

namespace net.atos.daf.ct2.organizationservice.entity
{
    public class OrganizationRequest
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
        public DateTime? reference_date { get; set; }
        public bool optout_status { get; set; }
        public DateTime? optout_status_changed_date { get; set; }
        //  public bool is_active  { get; set; }  

    }
}
