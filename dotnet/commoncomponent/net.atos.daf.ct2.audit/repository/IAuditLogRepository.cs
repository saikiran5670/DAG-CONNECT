using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit.entity;

namespace net.atos.daf.ct2.audit.repository
{
    public interface IAuditLogRepository
    {
         Task<int> AddLogs(AuditTrail auditTrail);
         IEnumerable<AuditTrail> GetAuditLogs(int Userorgid);
    }
}
