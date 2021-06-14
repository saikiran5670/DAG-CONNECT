using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.tcuprovisioningtest
{
    [TestClass]
    public class TestVehicleMethods
    {
        private readonly string _psqlconnstring;
        private readonly string _datamartpsqlconnstring;
        private readonly IDataAccess _dataacess = null;
        private readonly IDataMartDataAccess _datamartDataacess = null;
        private readonly IAuditTraillib _auditlog = null;
        private readonly IAuditLogRepository _auditrepo = null;
        private readonly IVehicleRepository _vehiclerepo = null;

        public TestVehicleMethods()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettingsDevelopment.json", optional: true, reloadOnChange: true).Build();

            _psqlconnstring = config.GetSection("PSQL_CONNSTRING").Value;
            _datamartpsqlconnstring = config.GetSection("DATAMART_CONNECTION_STRING").Value;

            _dataacess = new PgSQLDataAccess(_psqlconnstring);
            _datamartDataacess = new PgSQLDataMartDataAccess(_datamartpsqlconnstring);
            _auditrepo = new AuditLogRepository(_dataacess);
            _auditlog = new AuditTraillib(_auditrepo);
            _vehiclerepo = new VehicleRepository(_dataacess, _datamartDataacess);
        }

        [TestMethod]
        public async Task TestVehicleUpdate()
        {
            var vin = "KLRAE75PC0E200148";
            VehicleManager vehicleManager = new VehicleManager(_vehiclerepo, _auditlog);

            var receivedVehicle = await GetVehicle(vin, vehicleManager);

            var vehicle = await UpdateVehicle(receivedVehicle, vehicleManager);

            Assert.IsNotNull(vehicle);

        }

        private VehicleFilter GetFilteredVehicle(string vin)
        {
            VehicleFilter vehicleFilter = new VehicleFilter
            {
                OrganizationId = 0,
                VIN = vin,
                VehicleId = 0,
                VehicleGroupId = 0,
                AccountId = 0,
                FeatureId = 0,
                VehicleIdList = "",
                Status = 0,
                AccountGroupId = 0,
            };
            return vehicleFilter;
        }

        private async Task<Vehicle> GetVehicle(string vin, IVehicleManager vehicleManager)
        {
            try
            {
                VehicleFilter vehicleFilter = GetFilteredVehicle(vin);
                Vehicle receivedVehicle = null;
                IEnumerable<Vehicle> vehicles = await vehicleManager.Get(vehicleFilter);

                foreach (Vehicle vehicle in vehicles)
                {
                    receivedVehicle = vehicle;
                    break;
                }

                return receivedVehicle;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task<Vehicle> UpdateVehicle(Vehicle receivedVehicle, VehicleManager vehicleManager)
        {

            Vehicle veh;
            try
            {
                veh = await vehicleManager.Update(receivedVehicle);

            }
            catch (Exception ex)
            {
                string messageError = ex.Message;
                throw;
            }
            return veh;
        }


    }
}
