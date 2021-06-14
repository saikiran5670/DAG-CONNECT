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
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            string dataMartConnectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=vehicledatamart;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
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