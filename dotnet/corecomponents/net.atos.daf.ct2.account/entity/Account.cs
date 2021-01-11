using System;

namespace net.atos.daf.ct2.account
{
    public class Account
    {
        public int Id { get; set; }        
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public AccountType AccountType { get; set; }
        public int Organization_Id { get; set; } 
        public int Account_OrgId { get; set; } 
        public DateTime StartDate { get; set; }   
        public DateTime ? EndDate { get; set; }  
        public bool Active { get; set; }    
        
    }
}
