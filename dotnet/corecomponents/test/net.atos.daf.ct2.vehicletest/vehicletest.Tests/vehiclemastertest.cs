using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.data;

namespace vehicletest.Tests
{
    [TestClass]
    public class vehicletest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess dataAccess;
        private readonly IVehicleManagement _vehicleManagement;
        private readonly IVehicleRepository _vehicleRepository;
        
        [TestMethod]
        public void TestMethod1()
        {
             _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
           .Build();
            // Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            dataAccess = new PgSQLDataAccess(connectionString);
             _vehicleRepository = new VehicleRepository(dataAccess);
            _vehicleManagement=new VehicleManagement(_vehicleRepository);
        
        }

        [TestCategory("Unit-Test-Case 1")]
        [Description("Test method to create new vehicle")]
        [TestMethod]
        public void CreateVehicle()
        {
           Vehicle Objvehicle=new Vehicle();

           Objvehicle.OrganizationId=1;
           Objvehicle.Name="Vehicle 1";
           Objvehicle.VIN="435";
           Objvehicle.RegistrationNo="123";
           Objvehicle.ManufactureDate=DateTime.Now;
           Objvehicle.ChassisNo="123545";
           Objvehicle.StatusDate=DateTime.Now;


            var resultvehicle =  _vehicleManagement.Create(vehicle).Result;
            Assert.IsNotNull(resultvehicle);
            Assert.IsTrue(resultvehicle > 0 || resultvehicle == 0);
        }
    }
}
