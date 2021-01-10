using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using System.Linq;

namespace net.atos.daf.ct2.vehicle.test
{
    [TestClass]
    public class vehiclerepositorytest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly IVehicleRepository _vehicleRepository;

        public vehiclerepositorytest()
        {
              string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _vehicleRepository = new VehicleRepository(_dataAccess);
            
        }

        [TestMethod]
        public void CreateVehicle()
        {
            Vehicle Objvehicle = new Vehicle();

            Objvehicle.Organization_Id = 1;
            Objvehicle.Name = "Vehicle 1";
            Objvehicle.VIN = "435";
            Objvehicle.License_Plate_Number = "123";
            Objvehicle.ManufactureDate = DateTime.Now;
            Objvehicle.ChassisNo = "123545";
            Objvehicle.Status_Changed_Date = DateTime.Now;
            Objvehicle.Status=VehicleStatusType.OptIn;
            Objvehicle.Termination_Date=DateTime.Now;
            var resultvehicle = _vehicleRepository.Create(Objvehicle).Result;
            Assert.IsNotNull(resultvehicle);
            Assert.IsTrue(resultvehicle.ID > 0);

        }

        [TestMethod]
        public void GetVehicle()
        {
            VehicleFilter ObjFilter=new VehicleFilter ();
            ObjFilter.OrganizationId=1;
            
            var resultvehicleList = _vehicleRepository.Get(ObjFilter).Result;
            Assert.IsNotNull(resultvehicleList);
            Assert.IsTrue(resultvehicleList.Count() > 0);

        }
    }
}
