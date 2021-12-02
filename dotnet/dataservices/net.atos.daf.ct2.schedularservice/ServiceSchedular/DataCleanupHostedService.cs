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
        private readonly PurgingConfiguration _purgingConfiguration;
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
            _purgingConfiguration = new PurgingConfiguration();
            configuration.GetSection("PurgingConfiguration").Bind(_purgingConfiguration);
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");
            Task deq = Task.Run(async () =>
            {
                // _server.Start();           
                while (true)
                {

                    await DeleteDataFromTable();
                    Console.Write("****************************************Wait for 5 min*******************************************************");
                    await Task.Delay(_purgingConfiguration.ThreadSleepTimeInSec); // 5 mins sleep mode
                    Console.Write("****************************************Wait finish for 5 min*******************************************************");
                }
            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // httpClient.Dispose();
            return Task.CompletedTask;
        }
        public async Task DeleteDataFromTable()
        {
            int rowCount = 0;
            var attempts = 0;
            var tokenSource2 = new CancellationTokenSource();
            var state = string.Empty;
            var dataCleanupConfigurations = _dataCleanupManager.GetDataPurgingConfiguration().Result;

            var masterConnectionString = _configuration.GetConnectionString("ConnectionString");
            var datamartConnectionString = _configuration.GetConnectionString("DataMartConnectionString");
            Parallel.ForEach(dataCleanupConfigurations, new ParallelOptions() { MaxDegreeOfParallelism = datamartConnectionString.Length, CancellationToken = new CancellationToken() }, async node =>
            {
                using (CancellationTokenSource cancel = new CancellationTokenSource())
                {
                    var purgeSatrtTime = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString());
                    DataPurgingTableLog logData = new DataPurgingTableLog();

                    while (true)
                    {

                        try
                        {
                            var connString = node.DatabaseName != "dafconnectmasterdatabase" ? datamartConnectionString : masterConnectionString;

                            rowCount = await _dataCleanupManager.DeleteDataFromTables(connString, node);

                            state = rowCount == 0 ? "N" : "O";
                            logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                            await _dataCleanupManager.CreateDataPurgingTableLog(logData, masterConnectionString);
                            //if (rowCount >= 0)
                            //{
                            //     state = "O";
                            //    _logger.Info("RowCount is null");



                            //   // await _dataCleanupManager.CreateDataPurgingTableLog(logData, masterConnectionString);
                            //}
                            break;
                        }

                        catch (PostgresException neEx)
                        {

                            state = GetExceptionCode(neEx.Code.ToString());
                            if (attempts >= _purgingConfiguration.RetryCount)
                            {
                                logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData, masterConnectionString);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            state = e.Message.ToString() == "The operation has timed out." ? "T" : "F";
                            _logger.Info("Data purge failed");
                            _logger.Error(null, e);
                            logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                            attempts++;
                            if (attempts >= _purgingConfiguration.RetryCount)
                            {
                                logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData, masterConnectionString);
                                break;
                            }
                        }



                    }
                }
            });
        }
        private string GetExceptionCode(string errorCode)
        {
            var state = string.Empty;
            var sqlError = PgSqlErrorCodes.pgSQLErrorCodes.Any(x => x == errorCode);
            var connErros = PgSqlErrorCodes.connectionErrorCodes.Any(x => x == errorCode);
            var timeoutErrors = PgSqlErrorCodes.sessionTimeoutCodes.Any(x => x == errorCode);
            if (PgSqlErrorCodes.pgSQLErrorCodes.Any(x => x == errorCode))
            {
                state = "S";
            }
            else if (PgSqlErrorCodes.connectionErrorCodes.Any(x => x == errorCode))
            {
                state = "C";
            }
            else if (PgSqlErrorCodes.sessionTimeoutCodes.Any(x => x == errorCode))
            {
                state = "T";
            }
            return state;
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
