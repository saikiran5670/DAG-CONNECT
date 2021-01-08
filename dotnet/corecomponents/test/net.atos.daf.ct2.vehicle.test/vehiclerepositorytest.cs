using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;

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
              string connectionString = "Server = 127.0.0.1; Port = 5432; Database = MyWebAPi; User Id = postgres; Password = postgres; CommandTimeout = 90;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _vehicleRepository = new VehicleRepository(_dataAccess);
            
        }

        [TestMethod]
        public void CreateVehicle()
        {
            Vehicle Objvehicle = new Vehicle();

            Objvehicle.OrganizationId = 1;
            Objvehicle.Name = "Vehicle 1";
            Objvehicle.VIN = "435";
            Objvehicle.RegistrationNo = "123";
            Objvehicle.ManufactureDate = DateTime.Now;
            Objvehicle.ChassisNo = "123545";
            Objvehicle.StatusDate = DateTime.Now;
            Objvehicle.Status=VehicleStatusType.OptIn;
            Objvehicle.TerminationDate=DateTime.Now;
            var resultvehicle = _vehicleRepository.Create(Objvehicle).Result;
            Assert.IsNotNull(resultvehicle);
            Assert.IsTrue(resultvehicle.ID > 0);

        }
    }
}
