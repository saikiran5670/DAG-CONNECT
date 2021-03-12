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
            

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            TCUProvisioningDataProcess provisionVehicle = new TCUProvisioningDataProcess(log);
            provisionVehicle.readTCUProvisioningData();

        }
    }
}
