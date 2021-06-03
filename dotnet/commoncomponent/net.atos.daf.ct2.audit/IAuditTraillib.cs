using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.audit
{
    public interface IAuditTraillib
    {
        Task<int> AddLogs(AuditTrail auditTrail);
        Task<IEnumerable<AuditTrail>> GetAuditLogs(int performed_by, string component_name);
        Task<int> AddLogs(DateTime Created_at, DateTime Performed_at, int Performed_by, string Component_name, string Service_name, AuditTrailEnum.Event_type Event_type, AuditTrailEnum.Event_status Event_status, string Message, int Sourceobject_id, int Targetobject_id, string Updated_data, int roleid, int organizationid);
        Task<int> AddLogs(DateTime Created_at, DateTime Performed_at, int Performed_by, string Component_name, string Service_name, AuditTrailEnum.Event_type Event_type, AuditTrailEnum.Event_status Event_status, string Message, int Sourceobject_id, int Targetobject_id, string Updated_data);
    }
}
