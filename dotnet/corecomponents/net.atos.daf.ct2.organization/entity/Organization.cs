using System;

namespace net.atos.daf.ct2.organization.entity
{
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
        public long OptOutStatusChangedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string Referenced { get; set; }
        public int PreferenceId { get; set; }
        public string VehicleDefaultOptIn { get; set; }
        public string DriverDefaultOptIn { get; set; }

    }
}
