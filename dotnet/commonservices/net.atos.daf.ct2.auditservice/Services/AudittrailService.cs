using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.auditservice;

namespace net.atos.daf.ct2.auditservice.Services
{
    
    public class AudittrailService : AuditService.AuditServiceBase
    {
        private readonly ILogger<AudittrailService> _logger;
        
        private readonly IAuditLogRepository _IAuditLogRepository;
        
        private readonly IAuditTraillib _AuditTrail;
        public AudittrailService(ILogger<AudittrailService> logger, IAuditTraillib AuditTrail)
        {
            _logger = logger;
             _AuditTrail = AuditTrail;
        }

         
        public override Task<AuditResponce> Addlogs(AuditRecord request, ServerCallContext context)
        {
            try
            {
                AuditTrail logs= new AuditTrail();
                logs.Created_at= DateTime.Now;
                logs.Performed_at = DateTime.Now;
                logs.Performed_by=request.PerformedBy;
                logs.Component_name=request.ComponentName;
                logs.Service_name = request.ServiceName;                
                logs.Event_type=  AuditTrailEnum.Event_type.CREATE;
                logs.Event_status = AuditTrailEnum.Event_status.SUCCESS;  
                logs.Message = request.Message;  
                logs.Sourceobject_id = request.SourceobjectId;  
                logs.Targetobject_id = request.TargetobjectId;  
                logs.Updated_data = null;     
                var result = _AuditTrail.AddLogs(logs).Result;

                // var result = _logs.AddLogs(DateTime.Now,DateTime.Now,2,"Test2","Test",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null).Result;
                 return Task.FromResult(new AuditResponce
                {
                    Statuscode = "Success",
                    Message = "Log Added " 
                });
            }
            catch (Exception ex)
            {
                 return Task.FromResult(new AuditResponce
                {
                    Statuscode = "Error",
                    Message = ex.ToString()
                });

            }
               

           
        }

    }
}
