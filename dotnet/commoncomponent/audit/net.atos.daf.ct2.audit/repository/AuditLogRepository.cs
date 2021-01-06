using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.data;
using Dapper;

namespace net.atos.daf.ct2.audit.repository
{
    public class AuditLogRepository:IAuditLogRepository
    {
        private readonly IConfiguration  _config;
        private readonly IDataAccess dataAccess;
       
       public AuditLogRepository(IDataAccess _dataAccess) 
        {
            //_config = new ConfigurationBuilder()
           //.AddJsonFile("appsettings.Test.json")
           //.Build();
           // Get connection string
           //var connectionString = _config.GetConnectionString("DevAzure");
           //dataAccess = new PgSQLDataAccess(connectionString);
           dataAccess=_dataAccess;
        }
       public int AddLogs(AuditLogEntity auditLog)
       {
           try
           {
            var parameter = new DynamicParameters();
             parameter.Add("@userorgid", auditLog.Userorgid);
             parameter.Add("@eventid", auditLog.EventID);
             parameter.Add("@activitydescription",  auditLog.ActivityDescription);             
             parameter.Add("@eventperformed",  auditLog.EventPerformed);             
             parameter.Add("@eventtime",  DateTime.UtcNow);
             parameter.Add("@component",  auditLog.Component);
             parameter.Add("@eventstatus", auditLog.EventStatus);
             parameter.Add("@createddate",  DateTime.UtcNow);
             parameter.Add("@createdby",  auditLog.LoggedUserID);
                         
            return dataAccess.QuerySingle<int>("INSERT INTO dafconnectmaster.auditlog (userorgid, eventid, eventperformed, activitydescription, component, eventtime, eventstatus, createddate, createdby) VALUES(@userorgid, @eventid, @eventperformed, @activitydescription, @component, @eventtime, @eventstatus, @createddate, @createdby) RETURNING auditlogid",parameter);
           }
           catch(Exception ex)
           {
               throw ex;
               
           }
                  
       }

       public IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid)
        {
            try
            {
                return dataAccess.Query<AuditLogEntity>("SELECT userorgid, eventid, eventperformed, activitydescription, component, eventtime, eventstatus, createddate, createdby FROM dafconnectmaster.auditlog WHERE userorgid=@userorgid ", new { userorgid = @Userorgid });
            }
            catch (System.Exception)
            {

                throw;
            }

        }

  }
}

