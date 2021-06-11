using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit.entity;

namespace net.atos.daf.ct2.audit.repository
{
    public interface IAuditLogRepository
    {
        Task<int> AddLogs(AuditTrail auditTrail);
        Task<IEnumerable<AuditTrail>> GetAuditLogs(int performed_by, string component_name);
    }
}
