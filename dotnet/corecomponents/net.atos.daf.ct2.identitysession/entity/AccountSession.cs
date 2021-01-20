using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.identitysession.entity
{
    public class AccountSession
    {
        public int Id { get; set; }
         public int AccountId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public int LastSessionRefresh { get; set; }
        public int SessionStartedAt { get; set; }
        public int SessionExpiredAt { get; set; }
        public int CreatedAt { get; set; }
    }
}