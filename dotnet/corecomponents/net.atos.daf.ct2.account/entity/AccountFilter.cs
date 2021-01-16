using System;
using net.atos.daf.ct2.account.ENUM;
namespace net.atos.daf.ct2.account.entity
{
    public class AccountFilter
    {
         public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Email { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountIds { get; set; }
    }    
}
