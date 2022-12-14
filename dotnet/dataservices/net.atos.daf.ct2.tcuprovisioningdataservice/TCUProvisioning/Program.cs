using System.IO;
using System.Reflection;
using System.Threading.Tasks;
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
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It is kept for future modification")]
        static async Task Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string psqlConnString = config.GetSection("psqlconnstring").Value;

            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);

            TCUProvisioningDataProcess provisionVehicle = new TCUProvisioningDataProcess(_log, audit, config);
            await provisionVehicle.ReadTCUProvisioningDataAsync();

        }
    }
}
