using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;

namespace net.atos.daf.ct2.tcudataservice
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static async System.Threading.Tasks.Task Main()
        {


            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            TCUProvisioningDataProcess tcuProvisioningDataProcess = new TCUProvisioningDataProcess(_log, config);
            await tcuProvisioningDataProcess.ReadTcuProvisioningData();
        }
    }
}
