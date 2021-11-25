using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.schedular;
using net.atos.daf.ct2.schedular.repository;

namespace net.atos.daf.ct2.Schedular.test
{
    [TestClass]
    public class SchedularTest
    {
        private readonly IConfiguration _config = null;
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _datamartDataacess;
        private readonly IDataCleanupManager _dataCleanupManager;
        private readonly DataCleanupRepository _dataCleanupRepository;
        public SchedularTest()
        {
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            string datamartconnectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-datamart.postgres.database.azure.com;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _config = new ConfigurationBuilder().Build();
            _dataAccess = new PgSQLDataAccess(connectionString);
            _datamartDataacess = new PgSQLDataMartDataAccess(datamartconnectionString);
            _dataCleanupRepository = new DataCleanupRepository(_dataAccess, _datamartDataacess);
            // _dataCleanupManager = new NotificationIdentifierManager(_notificationIdentifierRepository);
        }
        [TestMethod]
        public void GeDataCleanupConfigurationTest()
        {
            var result = _dataCleanupRepository.DataPurging(new schedular.entity.DataCleanupConfiguration()).Result;
            // var purgingConfigdata = result.GroupBy(u => u.ColumnName).Select(grp => grp.ToList()).ToList();
            Assert.IsTrue(result != null);
        }
    }
}
