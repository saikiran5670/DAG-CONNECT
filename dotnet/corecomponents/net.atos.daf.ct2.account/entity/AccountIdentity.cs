using System;
using System.Collections.Generic;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.account.entity
{
    public class AccountIdentity
    {
        public bool Authenticated { get; set; }
        public Account accountInfo { get; set; }
        public List<KeyValue> AccountOrganization {get;set;}
        public List<AccountOrgRole> AccountRole {get;set;}
    }
}
