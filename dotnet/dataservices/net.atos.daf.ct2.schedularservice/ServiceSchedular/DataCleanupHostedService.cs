using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.schedular;
using net.atos.daf.ct2.schedular.entity;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
using Npgsql;

namespace net.atos.daf.ct2.schedularservice.ServiceSchedular
{
    public class DataCleanupHostedService : IHostedService
    {
        private readonly ILog _logger;
        private readonly Server _server;
        private readonly IDataCleanupManager _dataCleanupManager;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IConfiguration _configuration;
        private readonly Pugingefiguration _pugingefiguration;
        private readonly List<DataCleanupConfiguration> _dataCleanupConfigurations;

        public DataCleanupHostedService(IDataCleanupManager dataCleanupManager, Server server, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _dataCleanupManager = dataCleanupManager;
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
            this._configuration = configuration;
            _dataCleanupConfigurations = new List<DataCleanupConfiguration>();
            _pugingefiguration = new Pugingefiguration();
            configuration.GetSection("DataCleanupConfiguration").Bind(_pugingefiguration);
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");

            // _server.Start();           
            while (true)
            {

                DeleteDataFromTable();
                Thread.Sleep(_pugingefiguration.ThreadSleepTimeInSec); // 5 mins sleep mode
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // httpClient.Dispose();
            return Task.CompletedTask;
        }
        public void DeleteDataFromTable()
        {
            int rowCount = 0;
            var attempts = 0;
            var tokenSource2 = new CancellationTokenSource();
            var dataCleanupConfigurations = _dataCleanupManager.GetDataPurgingConfiguration().Result;
            Parallel.ForEach(dataCleanupConfigurations, new ParallelOptions() { MaxDegreeOfParallelism = 15, CancellationToken = new CancellationToken() }, async node =>
            {
                using (CancellationTokenSource cancel = new CancellationTokenSource())
                {
                    var purgeSatrtTime = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString());
                    DataPurgingTableLog logData = new DataPurgingTableLog();
                    //  var connString = string.Empty;
                    while (true)
                    {

                        try
                        {
                            var connString = node.DatabaseName != "dafconnectmasterdatabase" ? _configuration.GetConnectionString("DataMartConnectionString") :
                                                                                              _configuration.GetConnectionString("ConnectionString");
                            rowCount = await _dataCleanupManager.DeleteDataFromTables(connString, node);
                            if (rowCount >= 0)
                            {
                                var state = "O";
                                _logger.Info("RowCount is null");
                                logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData, connString);
                            }
                            break;
                        }
                        catch (Exception e)
                        {
                            attempts++;
                            var state = "S";
                            _logger.Info("Data purge failed");
                            _logger.Error(null, e);
                            logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                            if (attempts >= _pugingefiguration.RetryCount)
                            {
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData, string.Empty); //second parameter is not reqired
                                break;
                            }
                        }

                    }
                }
            });
        }
        private DataPurgingTableLog ToTableLog(DataCleanupConfiguration dataCleanupConfiguration, long purgeSatrtTime, int noOfRows, string state)
        {

            var logData = new DataPurgingTableLog()
            {

                PurgingStartTime = purgeSatrtTime,
                TableName = dataCleanupConfiguration.TableName,
                ColumnName = dataCleanupConfiguration.ColumnName,
                DatabaseName = dataCleanupConfiguration.DatabaseName,
                CreatedAt = dataCleanupConfiguration.CreatedAt,
                NoOfDeletedRecords = noOfRows,
                PurgingEndTime = UTCHandling.GetUTCFromDateTime(DateTime.Now),
                SchemaName = dataCleanupConfiguration.SchemaName,
                State = state,
                Duration = UTCHandling.GetUTCFromDateTime(DateTime.Now) - purgeSatrtTime
            };

            return logData;
        }

    }
}
