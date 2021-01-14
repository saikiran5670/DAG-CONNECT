using System;
using System.Collections.Generic;
namespace net.atos.daf.ct2.account
{
    public class AccountFilter
    {
         public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountIds { get; set; }
    }    
}
