using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using log4net;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.auditservice;
using net.atos.daf.ct2.portalservice.Entity.Audit;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class AuditHelper
    {
        private readonly AuditService.AuditServiceClient _auditService;
        private readonly ILog _logger;
        public AuditHelper(AuditService.AuditServiceClient auditService)
        {
            _auditService = auditService;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public HeaderObj GetHeaderData(HttpRequest request)
        {
            var headerObj = new HeaderObj();
            try
            {
                if (request != null)
                {
                    var Headers = request.Headers;
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    if (Headers.Any(item => item.Key == "headerObj"))
                    {
                        headerObj = JsonConvert.DeserializeObject<HeaderObj>(Headers["headerObj"], settings);
                    }
                    else if (Headers.Any(item => item.Key == "Headerobj"))
                    {
                        headerObj = JsonConvert.DeserializeObject<HeaderObj>(Headers["Headerobj"], settings);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while fetching request header object.", ex);
            }
            return headerObj;
        }

        public async Task<int> AddLogs(DateTime Performed_at, string Component_name, string Service_name, AuditTrailEnum.Event_type Event_type, AuditTrailEnum.Event_status Event_status, string Message, int Sourceobject_id, int Targetobject_id, string Updated_data, HttpRequest request)
        {
            AuditRecord logs = new AuditRecord();
            try
            {
                var headerData = GetHeaderData(request);
                int roleid = headerData.RoleId;
                int organizationid = headerData.OrgId;
                int Accountid = headerData.AccountId;

                logs.PerformedAt = Timestamp.FromDateTime(Performed_at.ToUniversalTime());
                logs.PerformedBy = Accountid;
                logs.ComponentName = Component_name;
                logs.ServiceName = Service_name;
                logs.Type = MapType(Event_type);
                logs.Status = MapStatus(Event_status);
                // logs.Event_type=  AuditTrailEnum.Event_type.CREATE; // (AuditTrailEnum.Event_type)Enum.Parse(typeof(AuditTrailEnum.Event_type), request.Type.ToString().ToUpper());
                // logs.Event_status =  AuditTrailEnum.Event_status.SUCCESS; 
                logs.Message = Message;
                logs.SourceobjectId = Sourceobject_id;
                logs.TargetobjectId = Targetobject_id;
                logs.UpdatedData = Updated_data;
                logs.RoleID = roleid;
                logs.OrganizationId = organizationid;
                AuditResponce auditresponse = await _auditService.AddlogsAsync(logs);

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while adding audit logs.", ex);
                return 1;
            }

        }

        public static int ToInt32(string value)
        {
            if (value == null)
                return 0;
            return int.Parse(value, (IFormatProvider)CultureInfo.CurrentCulture);
        }

        private static Event_type MapType(AuditTrailEnum.Event_type type)
        {
            switch (type)
            {
                case AuditTrailEnum.Event_type.LOGIN:
                    return Event_type.Login;
                case AuditTrailEnum.Event_type.CREATE:
                    return Event_type.Create;
                case AuditTrailEnum.Event_type.DELETE:
                    return Event_type.Delete;
                case AuditTrailEnum.Event_type.GET:
                    return Event_type.Get;
                case AuditTrailEnum.Event_type.UPDATE:
                    return Event_type.Update;
                default:
                    return Event_type.Create;
            }
        }

        private static Event_status MapStatus(AuditTrailEnum.Event_status status)
        {
            switch (status)
            {
                case AuditTrailEnum.Event_status.ABORTED:
                    return Event_status.Aborted;
                case AuditTrailEnum.Event_status.FAILED:
                    return Event_status.Failed;
                case AuditTrailEnum.Event_status.PENDING:
                    return Event_status.Pending;
                case AuditTrailEnum.Event_status.SUCCESS:
                    return Event_status.Success;
                default:
                    return Event_status.Success;
            }
        }
    }
}
