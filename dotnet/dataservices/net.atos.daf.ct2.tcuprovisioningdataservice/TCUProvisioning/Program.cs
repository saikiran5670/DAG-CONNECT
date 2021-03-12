using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace TCUProvisioning
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static async Task Main(string[] args)
        {
            

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            TCUProvisioningDataProcess provisionVehicle = new TCUProvisioningDataProcess(log);
            await provisionVehicle.readTCUProvisioningDataAsync();

        }
    }
}
