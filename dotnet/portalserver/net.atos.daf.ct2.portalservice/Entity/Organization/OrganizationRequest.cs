using System;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
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
        public string vehicle_default_opt_in { get; set; }
        public string driver_default_opt_in { get; set; }

    }

    public class Organization
    {
        public int Id { get; set; }
        public string OrganizationId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string AddressType { get; set; }
        public string AddressStreet { get; set; }
        public string AddressStreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public DateTime? reference_date { get; set; }
        public string VehicleDefaultOptIn { get; set; }
        public string DriverDefaultOptIn { get; set; }
    }
}
