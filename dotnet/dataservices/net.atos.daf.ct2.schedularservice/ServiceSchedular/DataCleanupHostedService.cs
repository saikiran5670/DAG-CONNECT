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
        private readonly ConcurrentQueue<DataCleanupConfiguration> _purgingTables;
        private readonly List<DataCleanupConfiguration> _dataCleanupConfigurations;
        private readonly NpgsqlConnection _dbConn;

        //  private readonly DataCleanupQueue _dataCleanupQueue;
        public DataCleanupHostedService(IDataCleanupManager dataCleanupManager, Server server, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _dataCleanupManager = dataCleanupManager;
            // _dataCleanupQueue = new DataCleanupQueue(_dataCleanupManager);
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
            this._configuration = configuration;
            _purgingTables = new ConcurrentQueue<DataCleanupConfiguration>();
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
                Thread.Sleep(_pugingefiguration.ThreadSleepTimeInSec); // 10 sec sleep mode
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
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData);
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
                                await _dataCleanupManager.CreateDataPurgingTableLog(logData);
                                break;
                            }
                        }

                    }
                }
            });
        }
        private DataPurgingTableLog ToTableLog(DataCleanupConfiguration dataCleanupConfiguration, long purgeSatrtTime, int noOfRows, string state)
        {

            var _logData = new DataPurgingTableLog()
            {

                PurgingStartTime = purgeSatrtTime,
                TableName = dataCleanupConfiguration.TableName,
                ColumnName = dataCleanupConfiguration.ColumnName,
                DatabaseName = dataCleanupConfiguration.DatabaseName,
                CreatedAt = dataCleanupConfiguration.CreatedAt,
                NoOfDeletedRecords = noOfRows,
                PurgingEndTime = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()),
                SchemaName = dataCleanupConfiguration.SchemaName,
                State = state,
                // Duration
            };

            return _logData;
        }


        // public async Task Worker(DataCleanupConfiguration op)
        public void Worker(DataCleanupConfiguration op)
        {
            Console.WriteLine("Worker {0} is processing item {1}", op.Id, op.ColumnName);
            try
            {
                var tries = 0;
                var maxRetryCount = 5;
                while (tries < maxRetryCount)
                {
                    try
                    {
                        tries++;
                        var noOfDeletedData = _dataCleanupManager.DataPurging(op);
                        Thread.Sleep(50000);
                        break;
                    }
                    catch (Exception)
                    {
                        if (tries > 3)

                            StopAsync(new CancellationToken()).ConfigureAwait(true); ;
                        throw;
                        //add log
                    }
                }
                //Write a logic to deque
                //write a logic to add log for success & retires count 
                //write a table specific logs e
            }
            catch (Exception ex)
            {
                // _log.Info("AddExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                // _log.Error(ex.ToString());
            }
            finally
            {
            }
            //logic to insert data into logtable
            Console.WriteLine("Worker {0} is stopping.", op.Id);
        }


    }
}
