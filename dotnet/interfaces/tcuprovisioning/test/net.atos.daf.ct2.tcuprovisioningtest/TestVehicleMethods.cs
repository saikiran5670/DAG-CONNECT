using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.tcucore;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.tcuprovisioningtest
{
    [TestClass]
    public class TestVehicleMethods
    {
        private string psqlconnstring;
        private string datamartpsqlconnstring;
        IDataAccess dataacess = null;
        IDataMartDataAccess datamartDataacess = null;    
        IConfiguration config = null;
        IAuditTraillib auditlog = null;
        IAuditLogRepository auditrepo = null;
        IVehicleRepository vehiclerepo = null;

        public TestVehicleMethods()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettingsDevelopment.json", optional: true, reloadOnChange: true).Build();

            psqlconnstring = config.GetSection("PSQL_CONNSTRING").Value;
            datamartpsqlconnstring = config.GetSection("DATAMART_CONNECTION_STRING").Value;

            dataacess = new PgSQLDataAccess(psqlconnstring);
            datamartDataacess = new PgSQLDataMartDataAccess(datamartpsqlconnstring);
            auditrepo = new AuditLogRepository(dataacess);
            auditlog = new AuditTraillib(auditrepo);
            vehiclerepo = new VehicleRepository(dataacess, datamartDataacess);
        }

        [TestMethod]
        public async Task TestVehicleUpdate()
        {
            var vin = "KLRAE75PC0E200148";
            VehicleManager vehicleManager = new VehicleManager(vehiclerepo, auditlog);

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
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task<Vehicle> UpdateVehicle(Vehicle receivedVehicle, VehicleManager vehicleManager)
        {         

            Vehicle veh = null;
            try
            {          
                veh = await vehicleManager.Update(receivedVehicle);
               
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                 throw;
            }
            return veh;
        }


    }
}
