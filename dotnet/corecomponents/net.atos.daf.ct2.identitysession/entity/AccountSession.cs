using System;

namespace net.atos.daf.ct2.identitysession.entity
{
    public class AccountSession
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public long LastSessionRefresh { get; set; }
        public long SessionStartedAt { get; set; }
        public long SessionExpiredAt { get; set; }
        public long CreatedAt { get; set; }
        public Guid Session_Id { get; set; }
    }
}