using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using System;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit.repository; 
using net.atos.daf.ct2.audit.entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
namespace net.atos.daf.ct2.Audittrail.test
{
    [TestClass]
    public class AuditTrailtest
    {
        private readonly IAuditTraillib _logs;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly IAuditLogRepository _IAuditLogRepository;
       
       public AuditTrailtest()
        {
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _IAuditLogRepository = new AuditLogRepository(_dataAccess);
             _logs = new AuditTraillib(_IAuditLogRepository);
            
            // _IAuditLogRepository = new AuditLogRepository(_dataAccess);
        }

        [TestMethod]
        public void AddLogs()
        {
            AuditTrail logs= new AuditTrail();
                logs.Created_at = DateTime.Now;
                logs.Performed_at = DateTime.Now;
                logs.Performed_by=1;
                logs.Component_name= "Test Component";
                logs.Service_name = "Audit Test";                
                logs.Event_type= "L";
                logs.Event_status = "S";  
                logs.Message = "Test unit test message";  
                logs.Sourceobject_id = 1;  
                logs.Targetobject_id = 2;  
                // logs.Updated_data =  @"{'FirstName':'Jignesh','LastName':'Trivedi'}";     
                 var result = _logs.AddLogs(logs);
                Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void AddLogsParam()
        {
              
                 var result = _logs.AddLogs(DateTime.Now,DateTime.Now,2,"Test","Test","L","S","Test",1,2,null);
                Assert.IsTrue(result > 0);
        }

    //      [TestCleanup]
    //    public static void CleanUpTests()
    //    {
    //        // remove resources used for this.
    //    }
    }
}
