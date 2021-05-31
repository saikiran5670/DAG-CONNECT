using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;

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
            AuditTrail logs = new AuditTrail();
            logs.Created_at = DateTime.Now;
            logs.Performed_at = DateTime.Now;
            logs.Performed_by = 1;
            logs.Component_name = "Test Component";
            logs.Service_name = "Audit Test";
            logs.Event_type = AuditTrailEnum.Event_type.CREATE;
            logs.Event_status = AuditTrailEnum.Event_status.SUCCESS;
            logs.Message = "Test unit test message";
            logs.Sourceobject_id = 1;
            logs.Targetobject_id = 2;
            // logs.Updated_data =  @"{'FirstName':'Jignesh','LastName':'Trivedi'}";     
            var result = _logs.AddLogs(logs).Result;
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void AddLogsParam()
        {

            var result = _logs.AddLogs(DateTime.Now, DateTime.Now, 2, "Test2", "Test", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Test", 1, 2, null).Result;
            Assert.IsTrue(result > 0);
        }

        //      [TestCleanup]
        //    public static void CleanUpTests()
        //    {
        //        // remove resources used for this.
        //    }
    }
}
