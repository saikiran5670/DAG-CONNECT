using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.kafkacdc.test
{
    [TestClass]
    public class AlertCDCTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _datamartDataacess;
        private readonly IVehicleAlertRefIntegrator _vehicleAlertRefIntegrator;
        private readonly IVehicleAlertRepository _vehicleAlertRepository;
        private readonly IConfiguration _configuration;
        private readonly KafkaConfiguration _kafkaConfig;

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
            _vehicleAlertRefIntegrator = new VehicleAlertRefIntegrator(_vehicleAlertRepository, _configuration);
        }

        [TestMethod]
        public void GetVehicleAlertRefFromAlertConfiguration()
        {
            var result = _vehicleAlertRefIntegrator.GetVehicleAlertRefFromAlertConfiguration(150).Result;
            Assert.IsTrue(result.Count > 0);
        }
    }
}
