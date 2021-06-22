using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using Npgsql;
using NpgsqlTypes;
using static Dapper.SqlMapper;

namespace net.atos.daf.ct2.audit.repository
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IDataAccess _dataAccess;
        public AuditLogRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<int> AddLogs(AuditTrail auditTrail)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(auditTrail.Created_at.ToString()));
                parameter.Add("@performed_at", UTCHandling.GetUTCFromDateTime(auditTrail.Performed_at.ToString()));
                parameter.Add("@performed_by", auditTrail.Performed_by);
                parameter.Add("@component_name", auditTrail.Component_name);
                parameter.Add("@service_name", auditTrail.Service_name);
                parameter.Add("@event_type", (char)auditTrail.Event_type);
                parameter.Add("@event_status", (char)auditTrail.Event_status);
                parameter.Add("@message", auditTrail.Message);
                parameter.Add("@sourceobject_id", auditTrail.Sourceobject_id);
                parameter.Add("@targetobject_id", auditTrail.Targetobject_id);
                parameter.Add("@updated_data", auditTrail.Updated_data);
                parameter.Add("@role_id", auditTrail.Role_Id);
                parameter.Add("@organization_id", auditTrail.Organization_Id);

                // return dataAccess.QuerySingle<int>("INSERT INTO dafconnectmaster.auditlog (userorgid, eventid, eventperformed, activitydescription, component, eventtime, eventstatus, createddate, createdby) VALUES(@userorgid, @eventid, @eventperformed, @activitydescription, @component, @eventtime, @eventstatus, @createddate, @createdby) RETURNING auditlogid",parameter);
                return await _dataAccess.QuerySingleAsync<int>("INSERT INTO auditlog.audittrail(created_at, performed_at, performed_by, component_name, service_name, event_type, event_status, message, sourceobject_id, targetobject_id, updated_data,role_id,organization_id) VALUES (@created_at, @performed_at, @performed_by, @component_name, @service_name, @event_type, @event_status, @message, @sourceobject_id, @targetobject_id, @updated_data,@role_id,@organization_id) RETURNING id", parameter);
            }
            catch (Exception)
            {
                throw;

            }

        }

        public async Task<IEnumerable<AuditTrail>> GetAuditLogs(int performed_by, string component_name)
        {
            try
            {

                var parameter = new DynamicParameters();
                parameter.Add("@performed_by", performed_by);
                parameter.Add("@component_name", component_name);
                List<AuditTrail> list = new List<AuditTrail>();
                var result = await _dataAccess.QueryAsync<dynamic>(@"SELECT id,created_at, performed_at, performed_by, component_name, service_name,  message, sourceobject_id, targetobject_id, updated_data
	                        FROM auditlog.audittrail where performed_by = @performed_by and component_name = @component_name order by 1 desc", parameter);
                foreach (var item in result)
                {
                    list.Add(Map(item));
                }
                return list;
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        private AuditTrail Map(dynamic record)
        {
            AuditTrail Entity = new AuditTrail();
            Entity.Audittrailid = record.id;
            if (record.created_at != null)
                Entity.Created_at = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.created_at, "UTC", "yyyy-MM-ddTHH:mm:ss"));
            if (record.performed_at != null)
                Entity.Performed_at = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.performed_at, "UTC", "yyyy-MM-ddTHH:mm:ss"));
            Entity.Performed_by = record.performed_by;
            Entity.Component_name = record.component_name;
            Entity.Service_name = record.service_name;
            Entity.Message = record.message;
            Entity.Sourceobject_id = record.sourceobject_id;
            Entity.Targetobject_id = record.targetobject_id;
            Entity.Updated_data = record.updated_data;
            return Entity;
        }

    }

    public class JsonParameter : ICustomQueryParameter
    {
        private readonly string _value;

        public JsonParameter(string value)
        {
            _value = value;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = new NpgsqlParameter(name, NpgsqlDbType.Json);
            parameter.Value = _value;

            command.Parameters.Add(parameter);
        }
    }
}

