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
            _fmsManager = new FmsManager(_rfmsRepository);
        }

        [TestMethod]
        public void GetVehiclePosition()
        {
            try
            {
                //dev1  vin = M4A1116
                var data = _fmsManager.GetVehiclePosition("M4A1116", "1632700800000").Result;
                Assert.IsNotNull(data);
            }
            catch (Exception ex)
            {
            }
        }
        [TestMethod]
        public void GetVehicleStatus()
        {
            try
            {
                //dev1  vin = M4A1116
                var data = _fmsManager.GetVehicleStatus("M4A1116", "1632700800000").Result;
                Assert.IsNotNull(data);
            }
            catch (Exception ex)
            {
            }
        }
    }
}