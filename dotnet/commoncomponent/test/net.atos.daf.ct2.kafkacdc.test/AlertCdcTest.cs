using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.kafkacdc.test
{
    [TestClass]
    public class AlertCDCTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _datamartDataacess;
        private readonly IAlertMgmAlertCdcManager _vehicleAlertRefManager;
        private readonly IAlertMgmAlertCdcRepository _vehicleAlertRepository;
        private readonly ILandmarkAlertCdcManager _landmarkrefmanager;
        private readonly ILandmarkAlertCdcRepository _landmarkrefrepo;
        private readonly IConfiguration _configuration;
        private readonly KafkaConfiguration _kafkaConfig;
        private readonly VehicleCdcManager _vehicleCdcManager;
        private readonly IVisibilityManager _visibilityManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IVehicleManagementAlertCDCManager _vehicleMgmAlertCdcManager;

        public AlertCDCTest()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            string connectionString = _configuration.GetConnectionString("DevAzure");
            string datamartconnectionString = _configuration.GetConnectionString("DevAzureDatamart");
            _dataAccess = new PgSQLDataAccess(connectionString);
            //_visibilityManager = new VisibilityManager(visibilityManager, vehicleManager);
            _datamartDataacess = new PgSQLDataMartDataAccess(datamartconnectionString);
            _vehicleAlertRepository = new AlertMgmAlertCdcRepository(_dataAccess, _datamartDataacess);
            _configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
            _vehicleAlertRefManager = new AlertMgmAlertCdcManager(_vehicleAlertRepository, _configuration, _visibilityManager);
            var vehicleCdcrepository = new VehicleCdcRepository(_dataAccess, _datamartDataacess);
            _vehicleCdcManager = new VehicleCdcManager(vehicleCdcrepository);
            var vehicleManagementAlertCDCRepository = new VehicleManagementAlertCDCRepository(_dataAccess, _datamartDataacess);
            //_vehicleMgmAlertCdcManager = new VehicleManagementAlertCDCManager(_vehicleAlertRepository, vehicleManagementAlertCDCRepository, _configuration);
            _landmarkrefrepo = new LandmarkAlertCdcRepository(_dataAccess, _datamartDataacess);
            _landmarkrefmanager = new LandmarkAlertCdcManager(_landmarkrefrepo, _vehicleAlertRepository, _configuration);
        }

        [TestMethod]
        public void AddAlert()
        {
            var result = _vehicleAlertRefManager.GetVehicleAlertRefFromAlertConfiguration(12, "I").Result;
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void UpdateAlert()
        {
            var result = _vehicleAlertRefManager.GetVehicleAlertRefFromAlertConfiguration(12, "U").Result;
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void Delete()
        {
            var result = _vehicleAlertRefManager.GetVehicleAlertRefFromAlertConfiguration(12, "D").Result;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LandmarkUpdate()
        {
            var result = _landmarkrefmanager.LandmarkAlertRefFromAlertConfiguration(12, "D", "P").Result;
            Assert.IsTrue(result);
        }

        //[TestMethod]
        //public void UpdateVehcle()
        //{
        //    var result = _vehicleMgmAlertCdcManager.GetVehicleAlertRefFromVehicleId(new List<int> { 27 }, "N", 36, 36, 120, new List<int> { 0 }).Result;
        //    Assert.IsTrue(result);
        //}
        [TestMethod]
        public async Task VehicleCdcProducer()
        {
            var kafkaConfig = new KafkaConfiguration()
            {
                CA_CERT_LOCATION = "./cacert.pem",
                EH_CONNECTION_STRING = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                EH_FQDN = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",//BrokerList
                EH_NAME = "ingress.atos.vehicle.cdc.json" //topic name
            };
            // var vCdcList = new List<VehicleCdc>() { new VehicleCdc() { FuelType = "B", Status = "C", Vid = "M4A1113", FuelTypeCoefficient = 0, Vin = "XLRAE75PC0E345556" } };
            var vehicleIds = new List<int>() { 10 };
            await _vehicleCdcManager.VehicleCdcProducer(vehicleIds, kafkaConfig);
            //  Assert.IsTrue(result != null);


        }
        [TestMethod]
        public void VehicleCdcConsumer()
        {
            var kafkaConfig = new KafkaConfiguration()
            {
                CA_CERT_LOCATION = "./cacert.pem",
                CONSUMER_GROUP = "cdcvehicleconsumer",
                EH_CONNECTION_STRING = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                EH_FQDN = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",//BrokerList
                EH_NAME = "ingress.atos.vehicle.cdc.json" //topic name
            };
            var result = _vehicleCdcManager.VehicleCdcConsumer(kafkaConfig);
            Assert.IsTrue(result != null);


        }


        #region VehicleCdc
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get VehicleBySubscriptionSet ")]
        [TestMethod]
        public async Task UnT_vehicle_VehicleManager_GetVehicleCdc()
        {
            List<int> vids = new List<int>() { 1 };
            var results = await _vehicleCdcManager.GetVehicleCdc(vids);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }
        #endregion

    }
}
