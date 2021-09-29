using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.fms.repository;
using net.atos.daf.ct2.fms.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using Microsoft.Extensions.Caching.Memory;

namespace net.atos.daf.ct2.rfms.test
{
    [TestClass]
    public class FmsTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private readonly IFmsRepository _rfmsRepository;
        private readonly IFmsManager _fmsManager;

        public FmsTest()
        {
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            string dataMartConnectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-datamart.postgres.database.azure.com;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _dataMartDataAccess = new PgSQLDataMartDataAccess(dataMartConnectionString);

            _rfmsRepository = new FmsRepository(_dataAccess, _dataMartDataAccess);
            //var vehicleRepo = new VehicleRepository(_dataAccess, _dataMartDataAccess);
            //var vehicleManager = new VehicleManager(vehicleRepo);

            _fmsManager = new FmsManager(_rfmsRepository);
        }

        [TestMethod]
        public void GetVehicleStatus()
        {
            RfmsVehicleStatusRequest rfmsVehicleRequest = new RfmsVehicleStatusRequest()
            {
                AccountId = 171,
                OrgId = 36,
                //Vin = "BLRAE75PC0E272200",
                //  StartTime = "2021-07-30T01:40:00.000Z",


            };

            var rfmsVehicleList = _fmsManager.GetRfmsVehicleStatus(rfmsVehicleRequest).Result;
            Assert.IsNotNull(rfmsVehicleList);


        }
    }
}