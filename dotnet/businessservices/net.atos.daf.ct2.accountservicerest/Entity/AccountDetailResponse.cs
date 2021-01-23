using System;

namespace net.atos.daf.ct2.accountservicerest
{
    public class AccountDetailsResponse
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
        public string Roles { get; set; }
        public string AccountGroups { get; set; }      
    }
}
 