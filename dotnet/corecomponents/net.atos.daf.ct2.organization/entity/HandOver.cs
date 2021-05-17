using System;

namespace net.atos.daf.ct2.organization.entity
{
    public class HandOver
    { 
        public string VIN { get; set; } 
        public string TCUID { get; set; } 
        public string TCUActivation { get; set; } 
        public DateTime ReferenceDateTime { get; set; } 
        public string CustomerID { get; set; } 
        public string CustomerName { get; set; } 
        public string Type { get; set; } 
        public string Street { get; set; } 
        public string StreetNumber { get; set; } 
        public string PostalCode { get; set; } 
        public string City { get; set; } 
        public string CountryCode { get; set; } 
        
        public string OwnerRelationship { get; set; } 
        public string OEMRelationship { get; set; } 
        public string OrgCreationPackage { get; set; } 
        public string DAFPACCAR  { get; set; } 

    }

     public class CustomerRequest
    { 
        public string CompanyType { get; set; } 
        public string CustomerID { get; set; } 
        public string CustomerName { get; set; } 
        public string AddressType { get; set; } 
        public string Street { get; set; } 
        public string StreetNumber { get; set; } 
        public string PostalCode { get; set; } 
        public string City { get; set; } 
        public string CountryCode { get; set; } 
        public DateTime ReferenceDateTime { get; set; }
        public string OwnerRelationship { get; set; }
        public string OEMRelationship { get; set; }
        public string OrgCreationPackage { get; set; }
        public string DAFPACCAR { get; set; }
    }
}
