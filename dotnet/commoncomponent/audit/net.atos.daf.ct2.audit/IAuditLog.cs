using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.audit
{
    public interface IAuditLog
    {
        int AddLogs(AuditLogEntity auditLog);
        IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid);
        int AddLogs(int userOrgId,int loggedUserId, int EventID,string EventPerformed,bool EventStatus,string Component,string ActivityDescription);
    }
}
