using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.organization.entity
{
    public class Address
    {
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Street { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string StreetNumber { get; set; }
        [StringLength(15, MinimumLength = 0)]
        public string PostalCode { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string City { get; set; }
        [StringLength(20, MinimumLength = 0)]
        public string CountryCode { get; set; }
    }
    public class Company
    {
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ID { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string Name { get; set; }
        public Address Address { get; set; }
        [Required]
        public string ReferenceDateTime { get; set; }
    }

    public class CompanyUpdatedEvent
    {
        [Required]
        public Company Company { get; set; }
    }

    public class Customer
    {
        [Required]
        public CompanyUpdatedEvent CompanyUpdatedEvent { get; set; }
    }
}

