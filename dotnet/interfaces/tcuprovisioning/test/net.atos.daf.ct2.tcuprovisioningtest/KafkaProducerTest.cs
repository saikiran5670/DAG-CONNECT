using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace net.atos.daf.ct2.tcuprovisioningtest
{
    [TestClass]
    public class KafkaProducerTest
    {
        readonly string _brokerList = string.Empty;
        readonly string _connectionString = string.Empty;
        readonly string _topic = string.Empty;
        readonly string _caCertLocation = string.Empty;

        public KafkaProducerTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettingsDevelopment.json", optional: true, reloadOnChange: true).Build();

            _brokerList = config.GetSection("EH_FQDN").Value;
            _connectionString = config.GetSection("EH_CONNECTION_STRING").Value;
            _topic = config.GetSection("EH_NAME").Value;
            _caCertLocation = config.GetSection("CA_CERT_LOCATION").Value;
        }

        [TestMethod]
        public void ProduceKafkaMessage()
        {
            //+ Arrange
            var jsonData = @"{'vin' : 'KLRAE75PC0E200444','deviceIdentifier' : 'BYXRS845213','deviceSerialNumber' : '9072435876203496','correlations' : {'deviceId' : 'DLDAE75PC0E348695','vehicleId' : '8756435876203'},'referenceDate':'2021-04-05T18:40:51'}";

            //+ Act
            Console.WriteLine("Initializing Producer");
            var output = Worker.Producer(_brokerList, _connectionString, _topic, _caCertLocation, jsonData);
            output.Wait();
            var result = output.Result;

            Console.WriteLine("Message Produced");

            //+ Assert
            Assert.IsTrue(result);
        }
    }
}
