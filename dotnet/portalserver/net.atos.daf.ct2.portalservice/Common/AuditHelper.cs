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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public HeaderObj GetHeaderData(HttpRequest request)
        {
            var headerObj = new HeaderObj();
            if (request != null)
            {
                var Headers = request.Headers;

                if (Headers.Any(item => item.Key == "headerObj"))
                {
                    headerObj = JsonConvert.DeserializeObject<HeaderObj>(Headers["headerObj"]);
                }
            }
            return headerObj;
        }
        public async Task<int> AddLogs(DateTime Created_at, DateTime Performed_at, string Component_name, string Service_name, AuditTrailEnum.Event_type Event_type, AuditTrailEnum.Event_status Event_status, string Message, int Sourceobject_id, int Targetobject_id, string Updated_data, HttpRequest request)
        {
            var headerData = GetHeaderData(request);
            int roleid = headerData.roleId;
            int organizationid = headerData.orgId;
            int Accountid = headerData.accountId;
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
            logs.UpdatedData = Updated_data;

            AuditResponce auditresponse = await _auditService.AddlogsAsync(logs);
            _logger.LogError("Logs running fine");
            return 0;
        }


        public static int ToInt32(string value)
        {
            if (value == null)
                return 0;
            return int.Parse(value, (IFormatProvider)CultureInfo.CurrentCulture);
        }
    }
    public class HeaderObj
    {
        public int roleId { get; set; }
        public int accountId { get; set; }
        public int orgId { get; set; }

    }
}
