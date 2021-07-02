using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.translation.repository;
namespace net.atos.daf.ct2.translation.test
{
    [TestClass]
    public class TransaltionTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly ITranslationRepository _translationRepository;

        public TransaltionTest()
        {
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _translationRepository = new TranslationRepository(_dataAccess);
            //  _logs = new AuditTraillib(_ITranslationRepository);

            // _IAuditLogRepository = new AuditLogRepository(_dataAccess);
        }
        [TestMethod]
        public void TestMethod1()
        {
            var result = _translationRepository.GetAllLanguageCode().Result;
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public void GetTranslationsByMenu()
        {
            var result = _translationRepository.GetTranslationsByMenu(24, "L", "EN-GB");

            Assert.IsTrue(result != null);
        }

        //[TestMethod]
        //public void GetLangagugeTranslationByKey()
        //{
        //        var result = _ITranslationRepository.GetLangagugeTranslationByKey("dlanguage_Lithuanian","D").Result;
        //        Assert.IsTrue(result.Count() > 0);
        //}



    }
}
