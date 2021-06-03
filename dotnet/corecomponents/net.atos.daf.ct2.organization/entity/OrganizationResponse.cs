using System.Collections.Generic;

namespace net.atos.daf.ct2.organization.entity
{
    public class OrganizationResponse
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
        public string ReferenceDate { get; set; }
        public char State { get; set; }
        public int PreferenceId { get; set; }
        public string VehicleDefaultOptIn { get; set; }
        public string DriverDefaultOptIn { get; set; }
        public List<OrganizationResponse> OrganizationList { get; set; }

    }
}
