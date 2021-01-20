using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.translation.repository; 
using net.atos.daf.ct2.translation.entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using net.atos.daf.ct2.translation.Enum;
namespace net.atos.daf.ct2.translation.test
{
    [TestClass]
    public class TransaltionTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly ITranslationRepository _ITranslationRepository;
       
       public TransaltionTest()
        {
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _ITranslationRepository = new TranslationRepository(_dataAccess);
            //  _logs = new AuditTraillib(_ITranslationRepository);
            
            // _IAuditLogRepository = new AuditLogRepository(_dataAccess);
        }
        [TestMethod]
        public void TestMethod1()
        {
                var result = _ITranslationRepository.GetAllLanguageCode().Result;
                Assert.IsTrue(result.Count() > 0);
        }
    }
}
