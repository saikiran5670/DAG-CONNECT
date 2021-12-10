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
                    Console.Write("Datapurging paused for 5 min");
                    await Task.Delay(_purgingConfiguration.ThreadSleepTimeInSec); // 5 mins sleep mode

                }
            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Timed Hosted Service is stopping.");
            return Task.CompletedTask;
        }
        public async Task DeleteDataFromTable()
        {
            int rowCount = 0;
            var attempts = 0;
            var workitemCount = 0;
            var tokenSource2 = new CancellationTokenSource();
            var state = string.Empty;
            var dataCleanupConfigurations = await _dataCleanupManager.GetDataPurgingConfiguration();

            var masterConnectionString = _configuration.GetConnectionString("ConnectionString");
            var datamartConnectionString = _configuration.GetConnectionString("DataMartConnectionString");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Parallel.ForEach(dataCleanupConfigurations, new ParallelOptions() { MaxDegreeOfParallelism = datamartConnectionString.Length, CancellationToken = cts.Token }, async node =>
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
                            _logger.Info("Data deleted successfully from" + node.DatabaseName + "." + node.TableName + "nuber of records deeted : " + rowCount);
                            break;
                        }

                        catch (PostgresException neEx)
                        {

                            attempts++;
                            state = GetExceptionCode(neEx.SqlState.ToString());
                            if (attempts >= _purgingConfiguration.RetryCount)
                            {
                                logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData, masterConnectionString);
                                break;
                            }
                        }
                        catch (OperationCanceledException e)
                        {
                            _logger.Info("Data purge entire operation was cancelled");
                            _logger.Error(null, e);

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
                                _logger.Info("Data purge looged to master.datapurgingtablelog table");
                                logData = ToTableLog(node, purgeSatrtTime, rowCount, state);
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData, masterConnectionString);
                                workitemCount++;
                                break;
                            }
                        }

                        if (workitemCount > _purgingConfiguration.WebServiceRetryCount)
                        {
                            _logger.Info("Data purge service stoped after " + _purgingConfiguration.CancellationTokenDuration + " workitem execution");
                            await StopAsync(new CancellationTokenSource(_purgingConfiguration.CancellationTokenDuration).Token);

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
            else if (PgSqlErrorCodes.authorizationCodes.Any(x => x == errorCode))
            {
                state = "A";
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
