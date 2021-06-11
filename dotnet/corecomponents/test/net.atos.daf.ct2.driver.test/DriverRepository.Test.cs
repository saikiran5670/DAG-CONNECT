//using Microsoft.Extensions.Configuration;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using net.atos.daf.ct2.audit;
//using net.atos.daf.ct2.data;
//using net.atos.daf.ct2.driver.entity;
//using System.Collections.Generic;

//namespace net.atos.daf.ct2.driver.test
//{
//    [TestClass]
//    public class DriverRepositoryTest
//    {
//        private readonly IDataAccess _dataAccess;
//        private readonly IConfiguration _config;
//        readonly IDriverRepository _driverRepository;
//        private readonly IAuditTraillib _auditlog;
//        private readonly IDriverManager _driverManager;
//        public DriverRepositoryTest()
//        {
//            // _config = new ConfigurationBuilder()
//            //  .AddJsonFile("appsettings.Test.json")
//            // .Build();
//            //Get connection string
//            // var connectionString = _config.GetConnectionString("DevAzure");
//            var connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";

//            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
//            _dataAccess = new PgSQLDataAccess(connectionString);
//            _driverRepository = new DriverRepository(_dataAccess);

//        }

//        [TestMethod]
//        public void ImportDrivers()
//        {
//            List<Driver> driverRequest = new List<Driver>();
//            for (var i = 0; i < 5; i++)
//            {
//                Driver ObjDriver = new Driver();
//                ObjDriver.Driver_id_ext = "drvid:" + i;
//                ObjDriver.Organization_id = 1;
//                ObjDriver.Salutation = "Mr";
//                ObjDriver.FirstName = "DrvFname" + i;
//                ObjDriver.LastName = "DrvLname" + i;
//                ObjDriver.DateOfBith = 1614587999000;
//                ObjDriver.Status = "O";
//                driverRequest.Add(ObjDriver);
//            }

//            var result = _driverRepository.ImportDrivers(driverRequest).Result;
//            Assert.IsTrue(result != null);
//        }


//        [TestMethod]
//        public void UpdateDrivers()
//        {
//            Driver ObjDriver = new Driver();
//            ObjDriver.Id = 1;
//            ObjDriver.Driver_id_ext = "drvid:";
//            ObjDriver.Organization_id = 1;
//            ObjDriver.Salutation = "Mr";
//            ObjDriver.FirstName = "UpdatedDRV";
//            ObjDriver.LastName = "UpdatedDRV";
//            ObjDriver.DateOfBith = 1614587999000;
//            ObjDriver.Status = "O";
//            var result = _driverRepository.UpdateDriver(ObjDriver).Result;
//            Assert.IsTrue(result != null);
//        }

//        [TestMethod]
//        public void GetAllDrivers(int OrganizatioId)
//        {
//            OrganizatioId = 1;
//            var result = _driverRepository.GetAllDrivers(OrganizatioId).Result;
//            Assert.IsTrue(result != null);
//        }

//        [TestMethod]
//        public void DeleteDrivers(int OrganizatioId, int DriverId)
//        {
//            OrganizatioId = 1;
//            DriverId = 1;
//            var result = _driverRepository.DeleteDriver(OrganizatioId, DriverId).Result;
//            Assert.IsTrue(result);
//        }

//        [TestMethod]
//        public void UpdateOptinOptout(int OrganizatioId, bool optoutStatus)
//        {
//            OrganizatioId = 1;
//            optoutStatus = true;
//            var result = _driverRepository.UpdateOptinOptout(OrganizatioId, optoutStatus).Result;
//            Assert.IsTrue(result);
//        }
//    }
//}
