using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
using Dapper;
using static Dapper.SqlMapper;
using Npgsql;
using NpgsqlTypes;

namespace net.atos.daf.ct2.audit.repository
{
    public class AuditLogRepository:IAuditLogRepository
    {
        private readonly IConfiguration  _config;
       
    //     private readonly IDataAccess dataAccess;
       
    //    public AuditLogRepository(IDataAccess _dataAccess) 
    //     {
    //         //_config = new ConfigurationBuilder()
    //        //.AddJsonFile("appsettings.Test.json")
    //        //.Build();
    //        // Get connection string
    //        //var connectionString = _config.GetConnectionString("DevAzure");
    //        //dataAccess = new PgSQLDataAccess(connectionString);
    //        dataAccess= _dataAccess;
    //     }
        private readonly IDataAccess dataAccess;
        public AuditLogRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
       public int AddLogs(AuditTrail auditTrail)
       {
           try
           {
            var parameter = new DynamicParameters();
             parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(auditTrail.Created_at.ToString()));
             parameter.Add("@performed_at", UTCHandling.GetUTCFromDateTime(auditTrail.Performed_at.ToString()));
             parameter.Add("@performed_by",  auditTrail.Performed_by);             
             parameter.Add("@component_name",  auditTrail.Component_name);             
             parameter.Add("@service_name",  auditTrail.Service_name);
             parameter.Add("@event_type",  (char)auditTrail.Event_type);
             parameter.Add("@event_status", (char)auditTrail.Event_status);
             parameter.Add("@message", auditTrail.Message);
             parameter.Add("@sourceobject_id",  auditTrail.Sourceobject_id);
             parameter.Add("@targetobject_id",  auditTrail.Targetobject_id);
             parameter.Add("@updated_data",  auditTrail.Updated_data);
             
            // return dataAccess.QuerySingle<int>("INSERT INTO dafconnectmaster.auditlog (userorgid, eventid, eventperformed, activitydescription, component, eventtime, eventstatus, createddate, createdby) VALUES(@userorgid, @eventid, @eventperformed, @activitydescription, @component, @eventtime, @eventstatus, @createddate, @createdby) RETURNING auditlogid",parameter);
            return dataAccess.QuerySingle<int>("INSERT INTO logs.audittrail(created_at, performed_at, performed_by, component_name, service_name, event_type, event_status, message, sourceobject_id, targetobject_id, updated_data) VALUES (@created_at, @performed_at, @performed_by, @component_name, @service_name, @event_type, @event_status, @message, @sourceobject_id, @targetobject_id, @updated_data) RETURNING id",parameter);
           }
           catch(Exception ex)
           {
               throw ex;
               
           }
                  
       }

       public IEnumerable<AuditTrail> GetAuditLogs(int Userorgid)
        {
            try
            {
                return dataAccess.Query<AuditTrail>("SELECT userorgid, eventid, eventperformed, activitydescription, component, eventtime, eventstatus, createddate, createdby FROM dafconnectmaster.auditlog WHERE userorgid=@userorgid ", new { userorgid = @Userorgid });
            }
            catch (System.Exception)
            {

                throw;
            }

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

