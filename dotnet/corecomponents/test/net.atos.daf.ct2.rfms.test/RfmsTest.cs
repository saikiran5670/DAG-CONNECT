using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.rfms.entity;


namespace net.atos.daf.ct2.rfms.test
{
    [TestClass]
    public class RfmsTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private readonly IRfmsRepository _rfmsRepository;

        public RfmsTest()
        {
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            string dataMartConnectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-datamart.postgres.database.azure.com;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _dataMartDataAccess = new PgSQLDataMartDataAccess(dataMartConnectionString);

            _rfmsRepository = new RfmsRepository(_dataAccess, _dataMartDataAccess);
        }

        [TestMethod]
        public void GetVehicles()
        {
            RfmsVehicleRequest rfmsVehicleRequest = new RfmsVehicleRequest();

            var rfmsVehicleList = _rfmsRepository.GetVehicles(rfmsVehicleRequest).Result;
            Assert.IsNotNull(rfmsVehicleList);


        }
    }
}