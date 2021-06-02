using System;

namespace net.atos.daf.ct2.organizationservice.entity
{
    public class OrganizationRequest
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string AddressType { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public bool OptoutStatus { get; set; }
        public DateTime? OptoutStatusChangedDate { get; set; }
        //  public bool is_active  { get; set; }  

    }
}
