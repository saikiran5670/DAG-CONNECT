using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit.entity;

namespace net.atos.daf.ct2.audit.repository
{
    public interface IAuditLogRepository
    {
        int AddLogs(AuditLogEntity auditLog);
         IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid);
    }
}
