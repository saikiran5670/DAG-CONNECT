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
using System.Text;
//using Google.Protobuf.WellKnownTypes;



namespace net.atos.daf.ct2.auditservice.Services
{
    
    public class AudittrailService : AuditService.AuditServiceBase
    {
        private readonly ILogger _logger;
        
        private readonly IAuditLogRepository _IAuditLogRepository;
        
        private readonly IAuditTraillib _AuditTrail;
        public AudittrailService(ILogger<AudittrailService> logger, IAuditTraillib AuditTrail)
        {
            _logger = logger;
             _AuditTrail = AuditTrail;
        }

         
        public override async Task<AuditResponce> Addlogs(AuditRecord request, ServerCallContext context)
        {
            try
            {
                AuditTrail logs= new AuditTrail();
                logs.Created_at= DateTime.Now;
                logs.Performed_at = request.PerformedAt.ToDateTime();
                logs.Performed_by=request.PerformedBy;
                logs.Component_name=request.ComponentName;
                logs.Service_name = request.ServiceName;                
                logs.Event_type=  (AuditTrailEnum.Event_type)Enum.Parse(typeof(AuditTrailEnum.Event_type), request.Type.ToString().ToUpper());
                logs.Event_status =   (AuditTrailEnum.Event_status)Enum.Parse(typeof(AuditTrailEnum.Event_status), request.Status.ToString().ToUpper());  
                // logs.Event_type=  AuditTrailEnum.Event_type.CREATE;
                // logs.Event_status =  AuditTrailEnum.Event_status.SUCCESS; 
                logs.Message = request.Message;  
                logs.Sourceobject_id = request.SourceobjectId;  
                logs.Targetobject_id = request.TargetobjectId;  
                logs.Updated_data = request.UpdatedData;     
                _logger.LogError("Logs running fine");
                int result = _AuditTrail.AddLogs(logs).Result;

                return await Task.FromResult(new AuditResponce
                {
                    Message = "Add Logs " + result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Audit Service:Addlogs : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new AuditResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Addlogs Faile due to - " + ex.Message
                });

            }
               

           
        }
       
        public override async Task<AuditLogResponse> GetAllLangaugecodes(AuditLogRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("All langauges method get");
                var auditlogs = await _AuditTrail.GetAuditLogs(request.PerformedBy,request.ComponentName);

                AuditLogResponse auditLogList = new AuditLogResponse();
                foreach (var item in auditlogs)
                {
                    var logs = new audittrailproperty();
                    logs.Audittrailid = item.Audittrailid;
                    //logs.CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                    //logs.PerformedAt = item.Performed_at;
                    logs.PerformedBy = item.Performed_by;
                    logs.ComponentName = item.Component_name == null ? "" : item.Component_name;
                    logs.ServiceName = item.Service_name == null ? "" : item.Service_name;
                    logs.Type = (Event_type)(int)item.Event_type;
                    logs.Status = (Event_status)(int)item.Event_status;
                    logs.Message = item.Message == null ? "" : item.Message;
                    logs.SourceobjectId = item.Sourceobject_id;
                    logs.TargetobjectId = item.Targetobject_id;
                    logs.UpdatedData = item.Updated_data == null ? "" : item.Updated_data;
                    auditLogList.Audittraillist.Add(logs);
                }
                return await Task.FromResult(auditLogList);

            }
            catch (Exception ex)
            {
                _logger.LogError("Audit Service:GetAllLangaugecodes : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new AuditLogResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetAllLangaugecodes Faile due to - " + ex.Message
                });
            }

        }

    }
}
