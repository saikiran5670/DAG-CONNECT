using System;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.account.entity
{
    public class AccountIdentity
    {
        public AccountToken AccountToken {get;set;}
        public AccountPreference AccountPreference {get;set;}
    }
}
