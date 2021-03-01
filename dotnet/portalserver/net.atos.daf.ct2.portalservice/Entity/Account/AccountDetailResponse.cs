using System;
using System.Collections.Generic;
namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountDetailsResponse
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
        public List<KeyValue> Roles { get; set; }
        public List<KeyValue> AccountGroups { get; set; }      
    }
    public class KeyValue
    {
        public int Id { get; set; }
        public string Name { get; set; }       
    }
}
 