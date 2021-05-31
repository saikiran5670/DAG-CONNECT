using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace net.atos.daf.ct2.tcuprovisioningtest
{
    [TestClass]
    public class KafkaProducerTest
    {
        IConfiguration config = null;
        string brokerList = string.Empty;
        string connectionString = string.Empty;
        string topic = string.Empty;
        string caCertLocation = string.Empty;

        public KafkaProducerTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettingsDevelopment.json", optional: true, reloadOnChange: true).Build();

            brokerList = config.GetSection("EH_FQDN").Value;
            connectionString = config.GetSection("EH_CONNECTION_STRING").Value;
            topic = config.GetSection("EH_NAME").Value;
            caCertLocation = config.GetSection("CA_CERT_LOCATION").Value;
        }

        [TestMethod]
        public void ProduceKafkaMessage()
        {
            //+ Arrange
            var jsonData = @"{'vin' : 'KLRAE75PC0E200444','deviceIdentifier' : 'BYXRS845213','deviceSerialNumber' : '9072435876203496','correlations' : {'deviceId' : 'DLDAE75PC0E348695','vehicleId' : '8756435876203'},'referenceDate':'2021-04-05T18:40:51'}";

            //+ Act
            Console.WriteLine("Initializing Producer");
            var output = Worker.Producer(brokerList, connectionString, topic, caCertLocation, jsonData);
            output.Wait();
            var result = output.Result;

            Console.WriteLine("Message Produced");

            //+ Assert
            Assert.IsTrue(result);
        }
    }
}
