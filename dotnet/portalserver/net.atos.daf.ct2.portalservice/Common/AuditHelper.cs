using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using net.atos.daf.ct2.auditservice;
using net.atos.daf.ct2.portalservice.Entity.Audit;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class AuditHelper
    {
        private readonly ILogger<AuditHelper> _logger;
        private readonly AuditService.AuditServiceClient _auditService;
        public AuditHelper(AuditService.AuditServiceClient auditService, ILogger<AuditHelper> logger)
        {
            _auditService = auditService;
            _logger = logger;

        }
       
        public async Task<int> AddLogs(DateTime Created_at, DateTime Performed_at,  string Component_name, string Service_name, AuditTrailEnum.Event_type Event_type, AuditTrailEnum.Event_status Event_status, string Message, int Sourceobject_id, int Targetobject_id, string Updated_data, HttpRequest request)
        {
            var Headers = request.Headers;
            int roleid = 0;
            int organizationid = 0;
            int Accountid = 0;
            if (Headers.Any(item=>item.Key == "roleid"))
            {
                roleid = AuditHelper.ToInt32(Headers["roleid"]);
            }
            if (Headers.Any(item => item.Key == "organizationid"))
            {
                organizationid = AuditHelper.ToInt32(Headers["organizationid"]);
            }
            if (Headers.Any(item => item.Key == "accountid"))
            {
                Accountid = AuditHelper.ToInt32(Headers["accountid"]);               
            }
            AuditRecord logs = new AuditRecord();            
            
            //logs.PerformedAt = DateTime.Now.Ticks;            
            logs.PerformedBy = Accountid;
            logs.ComponentName = Component_name;
            logs.ServiceName = Service_name;            
            //logs.Event_type = (AuditTrailEnum.Event_type)Enum.Parse(typeof(AuditTrailEnum.Event_type), Type.ToString().ToUpper());
            //logs.Event_status = (AuditTrailEnum.Event_status)Enum.Parse(typeof(AuditTrailEnum.Event_status), request.Status.ToString().ToUpper());
            // logs.Event_type=  AuditTrailEnum.Event_type.CREATE; // (AuditTrailEnum.Event_type)Enum.Parse(typeof(AuditTrailEnum.Event_type), request.Type.ToString().ToUpper());
            // logs.Event_status =  AuditTrailEnum.Event_status.SUCCESS; 
            logs.Message = Message;
            logs.SourceobjectId = Sourceobject_id;
            logs.TargetobjectId = Targetobject_id;
            logs.UpdatedData =Updated_data;

            AuditResponce auditresponse = await _auditService.AddlogsAsync(logs);
            _logger.LogError("Logs running fine");
            return  0;
        }


        public static int ToInt32(string value)
        {
            if (value == null)
                return 0;
            return int.Parse(value, (IFormatProvider)CultureInfo.CurrentCulture);
        }
    }
}
