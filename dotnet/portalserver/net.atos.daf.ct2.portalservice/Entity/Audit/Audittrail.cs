using System;

namespace net.atos.daf.ct2.portalservice.Entity.Audit
{
    public class Audittrail
    {
        public int Audittrailid { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Performed_at { get; set; }
        public int Performed_by { get; set; }
        public string Component_name { get; set; } ///Action
        public string Service_name { get; set; }
        public AuditTrailEnum.Event_type Event_type { get; set; }
        public AuditTrailEnum.Event_status Event_status { get; set; }
        public string Message { get; set; }
        public int Sourceobject_id { get; set; }
        public int Targetobject_id { get; set; }
        public string Updated_data { get; set; }
    }
}
