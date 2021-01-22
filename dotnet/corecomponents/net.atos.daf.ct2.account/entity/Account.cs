using System;
using net.atos.daf.ct2.account.ENUM;
namespace net.atos.daf.ct2.account.entity
{
    public class Account
    {
        public int Id { get; set; }        
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public long ? Dob { get; set; }
        public AccountType AccountType { get; set; }
        public int Organization_Id { get; set; } 
        public int Account_OrgId { get; set; } 
        public DateTime StartDate { get; set; }   
        public DateTime ? EndDate { get; set; }  
        public bool isDuplicate { get; set; }
        public bool isError { get; set; }
        // public bool Active { get; set; }    
        
    }
}
