using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using  net.atos.daf.ct2.audit.repository;
using System.Web;
namespace net.atos.daf.ct2.audit
{
    public class AuditTraillib:IAuditTraillib
    {
         private readonly IAuditLogRepository repository; // = new TranslationRepository();
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AuditTraillib(IAuditLogRepository _repository)
        {
            repository = _repository;
        }
        public async Task<int> AddLogs(AuditTrail auditTrail)
        {
            try
            {
                log.Info("Audit log add method called");
                             
                return await repository.AddLogs(auditTrail);
            }
            catch(Exception ex)
            {
                log.Info("Audit Log Add failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(auditTrail));
                log.Error(ex.ToString());
                return 0;
            }
                         
        }

        public async Task<int> AddLogs(DateTime Created_at,DateTime Performed_at,int Performed_by, string Component_name,string Service_name,AuditTrailEnum.Event_type Event_type,AuditTrailEnum.Event_status Event_status,string Message,int Sourceobject_id,int Targetobject_id,string Updated_data)
        {
            try
            {
                AuditTrail logs = new AuditTrail();
                logs.Created_at = Created_at;
                logs.Performed_at = DateTime.Now;
                logs.Performed_by=Performed_by;
                logs.Component_name= Component_name;
                logs.Service_name = Service_name;                
                logs.Event_type= Event_type;
                logs.Event_status = Event_status;  
                logs.Message = Message;  
                logs.Sourceobject_id = Sourceobject_id;  
                logs.Targetobject_id = Targetobject_id;  
                logs.Updated_data = Updated_data;                
                return await AddLogs(logs);
               
            }
            catch 
            {
                return 0;
            }
        }       

        public async Task<IEnumerable<AuditTrail>> GetAuditLogs(int performed_by)
        {
            return await repository.GetAuditLogs(performed_by);
        }
    }
}
