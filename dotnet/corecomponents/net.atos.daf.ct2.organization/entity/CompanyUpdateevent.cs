using System;

namespace net.atos.daf.ct2.organization.entity
{
        public class Address    {
        public string Type { get; set; } 
        public string Street { get; set; } 
        public string StreetNumber { get; set; } 
        public string PostalCode { get; set; } 
        public string City { get; set; } 
        public string CountryCode { get; set; } 
    }
     public class Company    {
        public string type { get; set; } 
        public string ID { get; set; } 
        public string Name { get; set; } 
        public Address Address { get; set; } 
        public string ReferenceDateTime { get; set; } 
    }

    public class CompanyUpdatedEvent    {
        public Company Company { get; set; } 
    }

    public class Customer {
        public CompanyUpdatedEvent CompanyUpdatedEvent { get; set; } 
    }
}

