using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace TCUProvisioning
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string brokerList = ConfigurationManager.AppSetting["EH_FQDN"];
            string connectionString = ConfigurationManager.AppSetting["EH_CONNECTION_STRING"];
            string topic = ConfigurationManager.AppSetting["EH_NAME"];
            string caCertLocation = ConfigurationManager.AppSetting["CA_CERT_LOCATION"];
            string consumerGroup = ConfigurationManager.AppSetting["CONSUMER_GROUP"];
            string psqlConnString = ConfigurationManager.AppSetting["psqlconnstring"];

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            Console.WriteLine("Initializing Consumer");
            //log.Info("Hello logging world!");
            await ProvisionVehicle.readTCUProvisioningData(brokerList, connectionString, consumerGroup, topic, caCertLocation, psqlConnString);
            Console.ReadKey();


        }
    }
}
