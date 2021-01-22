using System;
using  net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.organization.entity
{
    public class Organization:AccountPreference
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
        public long ReferencedDate { get; set; } 
         public bool OptOutStatus  { get; set; } 
         public long OptOutStatusChangedDate  { get; set; } 
         public bool IsActive  { get; set; }  

        public string Currency { get; set; }
        public string Timezone { get; set; }
        public string Timeformat { get; set; }   
        public string Vehicledisplay { get; set; }   
        public string Dateformat { get; set; }  
        public string LandingpageDisplay { get; set; }  
         public string Languagename { get; set; }    
         public string Unit { get; set; }      

        public string PrefType { get; set; }  

         public DateTime OptOutStatusDate  { get; set; }   
         public DateTime Referenced  { get; set; }   
           

    }
}
