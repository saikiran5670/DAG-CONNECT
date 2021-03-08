using System;

namespace TCUProvisioning
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string brokerList = ConfigurationManager.AppSetting["EH_FQDN"];
            string connectionString = ConfigurationManager.AppSetting["EH_CONNECTION_STRING"];
            string topic = ConfigurationManager.AppSetting["EH_NAME"];
            string caCertLocation = ConfigurationManager.AppSetting["CA_CERT_LOCATION"];
            string consumerGroup = ConfigurationManager.AppSetting["CONSUMER_GROUP"];
            string psqlConnString = ConfigurationManager.AppSetting["psqlconnstring"];

            Console.WriteLine("Initializing Consumer");
            await ProvisionVehicle.readTCUProvisioningData(brokerList, connectionString, consumerGroup, topic, caCertLocation, psqlConnString);
            Console.ReadKey();


        }
    }
}
