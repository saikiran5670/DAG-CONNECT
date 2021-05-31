using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;

namespace TCUProvisioning
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        static async System.Threading.Tasks.Task Main(string[] args)
        {


            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();


            string psqlConnString = config.GetSection("psqlconnstring").Value;
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);

            ProvisionVehicle provisionVehicle = new ProvisionVehicle(log, config, audit);
            await provisionVehicle.ReadTCUProvisioningData();

        }
    }
}
