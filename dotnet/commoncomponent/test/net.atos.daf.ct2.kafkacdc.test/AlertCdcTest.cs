using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.confluentkafka;

namespace net.atos.daf.ct2.kafkacdc.test
{
    [TestClass]
    public class AlertCDCTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _datamartDataacess;
        private readonly IVehicleAlertRefManager _vehicleAlertRefManager;
        private readonly IVehicleAlertRepository _vehicleAlertRepository;
        private readonly IConfiguration _configuration;
        private readonly KafkaConfiguration _kafkaConfig;
        private readonly VehicleCdcManager _vehicleCdcManager;

        public AlertCDCTest()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            string connectionString = _configuration.GetConnectionString("DevAzure");
            string datamartconnectionString = _configuration.GetConnectionString("DevAzureDatamart");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _datamartDataacess = new PgSQLDataMartDataAccess(datamartconnectionString);
            _vehicleAlertRepository = new VehicleAlertRepository(_dataAccess, _datamartDataacess);
            _configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
            _vehicleAlertRefManager = new VehicleAlertRefManager(_vehicleAlertRepository, _configuration);
            _vehicleCdcManager = new VehicleCdcManager();
        }

        [TestMethod]
        public void GetVehicleAlertRefFromAlertConfiguration()
        {
            var result = _vehicleAlertRefManager.GetVehicleAlertRefFromAlertConfiguration(150).Result;
            Assert.IsTrue(result.Count > 0);
        }
        [TestMethod]
        public void VehicleCdcProducer()
        {
            var _kafkaConfig1 = new KafkaConfiguration()
            {
                CA_CERT_LOCATION = "./cacert.pem",
                EH_CONNECTION_STRING = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                EH_FQDN = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",//BrokerList
                EH_NAME = "ingress.atos.vehicle.cdc.json" //topic name
            };
            var vCdcList = new List<VehicleCdc>() { new VehicleCdc() { FuelType = "B", Status = "C", Vid = "M4A1113", FuelTypeCoefficient = 0, Vin = "XLRAE75PC0E345556" } };
            var result = _vehicleCdcManager.VehicleCdcProducer(vCdcList, _kafkaConfig1);
            Assert.IsTrue(result != null);


        }
        [TestMethod]
        public void VehicleCdcConsumer()
        {
            var _kafkaConfig1 = new KafkaConfiguration()
            {
                CA_CERT_LOCATION = "./cacert.pem",
                CONSUMER_GROUP = "cdcvehicleconsumer",
                EH_CONNECTION_STRING = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                EH_FQDN = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",//BrokerList
                EH_NAME = "ingress.atos.vehicle.cdc.json" //topic name
            };
            var result = _vehicleCdcManager.VehicleCdcConsumer(_kafkaConfig1);


            Assert.IsTrue(result != null);


        }
    }
}
