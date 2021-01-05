using System;
using System.Collections.Generic;
using net.atos.daf.ct2.audit.entity;
using  net.atos.daf.ct2.audit.repository;

namespace net.atos.daf.ct2.audit
{
    public class AuditLog:IAuditLog
    {
         private readonly IAuditLogRepository repository; // = new TranslationRepository();

        public AuditLog(IAuditLogRepository _repository)
        {
            repository = _repository;
        }
        public int AddLogs(AuditLogEntity auditLog)
        {
            return repository.AddLogs(auditLog);
        }

        public int AddLogs(int userOrgId,int loggedUserId, int EventID,string EventPerformed,bool EventStatus,string Component,string ActivityDescription)
        {
            try
            {
                AuditLogEntity logs = new AuditLogEntity();
                logs.Userorgid = userOrgId;
                logs.LoggedUserID = loggedUserId;
                logs.EventID=EventID;
                logs.EventPerformed= EventPerformed;
                logs.EventStatus = EventStatus;                
                logs.Component=Component;
                logs.ActivityDescription = ActivityDescription;                
                return AddLogs(logs);
               
            }
            catch 
            {
                return 0;
            }
        }       

        public IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid)
        {
            return repository.GetAuditLogs(Userorgid);
        }
    }
}
