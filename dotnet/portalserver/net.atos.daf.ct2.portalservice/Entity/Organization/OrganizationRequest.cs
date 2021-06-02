using System;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class OrganizationRequest
    {
        public int Id { get; set; }
        public string Org_id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Address_type { get; set; }
        public string Street { get; set; }
        public string Street_number { get; set; }
        public string Postal_code { get; set; }
        public string City { get; set; }
        public string Country_code { get; set; }
        public DateTime? Reference_date { get; set; }
        public bool Optout_status { get; set; }
        public DateTime? Optout_status_changed_date { get; set; }
        //  public bool is_active  { get; set; }  
        public string Vehicle_default_opt_in { get; set; }
        public string Driver_default_opt_in { get; set; }

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
        public DateTime? Reference_date { get; set; }
        public string VehicleDefaultOptIn { get; set; }
        public string DriverDefaultOptIn { get; set; }
    }
}
