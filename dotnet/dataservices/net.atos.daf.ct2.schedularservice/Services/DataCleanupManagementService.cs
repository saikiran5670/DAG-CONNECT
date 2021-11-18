using System.Reflection;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.datacleanupservice;
using net.atos.daf.ct2.schedular;

namespace net.atos.daf.ct2.schedularservice.services
{
    public class DataCleanupManagementService : DataCleanupService.DataCleanupServiceBase
    {
        private readonly ILog _logger;
        private readonly IConfiguration _configuration;
        //private readonly ITripAlertManager _tripAlertManager;
        private readonly IDataCleanupManager _dataCleanupManager;
        public DataCleanupManagementService(IConfiguration configuration,
                                            IDataCleanupManager dataCleanupManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._configuration = configuration;
            //configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _dataCleanupManager = dataCleanupManager;
        }
    }
}
