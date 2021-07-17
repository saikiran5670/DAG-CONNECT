using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.tcucore;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using Newtonsoft.Json;

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

        private readonly string _dafurl;
        private readonly string _accessUrl;
        private readonly string _grantType;
        private readonly string _scope;
        private readonly string _clientId;
        private readonly string _clientSecret;

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

            _dafurl = config.GetSection("DAFURL").Value;
            _accessUrl = config.GetSection("ACCESS_TOKEN_URL").Value;
            _grantType = config.GetSection("GRANT_TYPE").Value;
            _scope = config.GetSection("CLIENT_SCOPE").Value;
            _clientId = config.GetSection("CLIENT_ID").Value;
            _clientSecret = config.GetSection("CLIENT_SECRET").Value;

        }

        [TestMethod]
        public async Task TestVehicleUpdate()
        {
            var vin = "KLRAE75PC0E200148";
            VehicleManager vehicleManager = new VehicleManager(_vehiclerepo);

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

        [TestMethod]
        public async Task TestDAFUrl()
        {
            string tcuDataSendJson = @"{'vin':'1FMFK20579EB76681','deviceIdentifier':'CCU_2740003513','deviceSerialNumber':'CCU_2740003513','correlations':{'deviceId':'CCU_2740003513','vehicleId':'baad0c0c-5f85-4a86-8f1c-8fe69d0c1c9d'}}";
            var client = await GetHttpClient();
            var data = new StringContent(tcuDataSendJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.BadRequest;

            response = await client.PostAsync(_dafurl, data);

            var responseCode = response.StatusCode;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.OK, responseCode);
        }

        private async Task<HttpClient> GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = await GetElibilityToken(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return client;

        }

        private async Task<TCUDataToken> GetElibilityToken(HttpClient client)
        {
            var form = new Dictionary<string, string>
                {
                    {"grant_type", _grantType},
                    {"scope", _scope},
                    {"client_id", _clientId},
                    {"client_secret", _clientSecret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(_accessUrl, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            TCUDataToken token = JsonConvert.DeserializeObject<TCUDataToken>(jsonContent);
            return token;
        }
    }
}
