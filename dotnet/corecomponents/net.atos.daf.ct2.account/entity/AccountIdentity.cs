using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.account.entity
{
    public class AccountIdentity
    {
        public string tokenIdentifier { get; set; }
        public Account accountInfo { get; set; }
        public List<KeyValue> AccountOrganization {get;set;}
        public List<AccountOrgRole> AccountRole {get;set;}
    }
}
