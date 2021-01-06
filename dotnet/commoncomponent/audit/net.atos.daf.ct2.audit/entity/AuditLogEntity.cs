using System;

namespace net.atos.daf.ct2.audit.entity
{
    public class AuditLogEntity
    {
        public int Userorgid { get; set; }
        public string Username { get; set; }       
        public int LoggedUserID { get; set; }
        public int EventID { get; set; } ///Action
        public string EventPerformed { get; set; } 
        public string  ActivityDescription { get; set; }
        public string Component { get; set; }
        public bool EventStatus { get; set; }
        public DateTime eventtime { get; set; }
        public DateTime createddate { get; set; }
        public int createdby { get; set; }
    }
}
