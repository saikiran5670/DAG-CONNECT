using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using Microsoft.Extensions.Caching.Memory;

namespace net.atos.daf.ct2.rfms.test
{
    [TestClass]
    public class RfmsTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private readonly IRfmsRepository _rfmsRepository;
        private readonly IRfmsManager _rfmsManager;

        public RfmsTest(IMemoryCache memoryCache)
        {
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            string dataMartConnectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-datamart.postgres.database.azure.com;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _dataMartDataAccess = new PgSQLDataMartDataAccess(dataMartConnectionString);

            _rfmsRepository = new RfmsRepository(_dataAccess, _dataMartDataAccess);
            var vehicleRepo = new VehicleRepository(_dataAccess, _dataMartDataAccess);
            var vehicleManager = new VehicleManager(vehicleRepo);

            _rfmsManager = new RfmsManager(_rfmsRepository, vehicleManager, memoryCache);
        }

        [TestMethod]
        public void GetVehicleStatus()
        {
            RfmsVehicleStatusRequest rfmsVehicleRequest = new RfmsVehicleStatusRequest()
            {
                AccountId =,
                OrgId =,
                Vin = 'BLRAE75PC0E272200',
                StartTime= "2021-07-30T01:40:00.000Z",


            };

            var rfmsVehicleList = _rfmsManager.GetRfmsVehicleStatus(rfmsVehicleRequest).Result;
            Assert.IsNotNull(rfmsVehicleList);


        }
    }
}