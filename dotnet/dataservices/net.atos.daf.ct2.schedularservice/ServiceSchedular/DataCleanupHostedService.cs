using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.schedular;
using net.atos.daf.ct2.schedular.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.schedularservice.ServiceSchedular
{
    public class DataCleanupHostedService : IHostedService
    {
        private readonly ILog _logger;
        private readonly Server _server;
        private readonly IDataCleanupManager _dataCleanupManager;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IConfiguration _configuration;
        private readonly DataCleanupConfiguration _dataCleanupConfiguration;

        public DataCleanupHostedService(IDataCleanupManager dataCleanupManager, Server server, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _dataCleanupManager = dataCleanupManager;
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
            this._configuration = configuration;

            _dataCleanupConfiguration = new DataCleanupConfiguration();
            configuration.GetSection("DataCleanupConfiguration").Bind(_dataCleanupConfiguration);

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");
            // _server.Start();           
            while (true)
            {
                await DeleteDataFromTable();
                Thread.Sleep(10000);// _dataCleanupConfiguration.ThreadSleepTimeInSec); // 10 sec sleep mode
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        private async Task DeleteDataFromTable()
        {
            try
            { }
            catch (Exception ex)
            {
                _logger.Error("Data Cleanup Host Service", ex);
                ///failed message is getting logged.
               // _logger.Info(JsonConvert.SerializeObject(tripAlert));
                //Need a discussion on handling failed kafka topic messages 
            }
        }





    }
}
