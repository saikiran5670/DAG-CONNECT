using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private DataCleanupConfiguration _dequeued;
        private readonly ConcurrentQueue<DataCleanupConfiguration> _purgingTables;
        private readonly List<DataCleanupConfiguration> _dataCleanupConfigurations;

        public DataCleanupHostedService(IDataCleanupManager dataCleanupManager, Server server, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _dataCleanupManager = dataCleanupManager;
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
            this._configuration = configuration;
            _purgingTables = new ConcurrentQueue<DataCleanupConfiguration>();
            _dataCleanupConfigurations = new List<DataCleanupConfiguration>();
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
            {
                Run();
                //TestConcurrentQueueAsync(); 
            }
            catch (Exception ex)
            {
                _logger.Error("Data Cleanup Host Service", ex);
                ///failed message is getting logged.
               // _logger.Info(JsonConvert.SerializeObject(tripAlert));
                //Need a discussion on handling failed kafka topic messages 
            }
        }

        public void TestConcurrentQueueAsync()
        {
            var purgingTable = _dataCleanupManager.GetDataPurgingConfiguration().Result;
            purgingTable.ForEach(_purgingTables.Enqueue);
            while (purgingTable.Any())
            {
                Task.Factory.StartNew(() =>
                {

                    if (_purgingTables.TryPeek(out _dequeued))
                    {
                        _dataCleanupManager.DataPurging(_dequeued);
                        // outputQueue.Enqueue(dequeued);
                    }
                });
            }



            //Task t1 = Task.Run(() => GetTbalesForConcurrentQueue(purgingTables));
            //Task.WaitAll(t1);

            //Console.WriteLine("Total orders before Dequeue are: {0}", purgingTables.Count);
            //DataCleanupConfiguration purgingItem = new DataCleanupConfiguration();

            //if (purgingTables.TryPeek(out purgingItem))
            //{
            //    Console.WriteLine("Order \"{0}\" has been retrieved", purgingItem);
            //    _dataCleanupManager.DataPurging(purgingItem);
            //}
            //else
            //{
            //    Console.WriteLine("Order queue is empty", purgingItem);
            //}

            //Console.WriteLine("Total orders after TryPeek are: {0}", purgingTables.Count);
        }



        //public void ExtractData()
        //{

        //    var purgingTables = _dataCleanupManager.GetDataPurgingConfiguration().Result;
        //    //        new List<DataCleanupConfiguration>() {
        //    //      new DataCleanupConfiguration() {  ColumnName="aaa",  DatabaseName="111"  },
        //    //      new DataCleanupConfiguration() {  ColumnName="sdsdsd",  DatabaseName="333333"  },
        //    //      new DataCleanupConfiguration() {  ColumnName="nnnnnnnnnnn",  DatabaseName="4444444"  },
        //    //};

        //    foreach (var item in purgingTables)
        //    {
        //        _purgingTables.Enqueue(item);
        //    }
        //    // perform data extraction
        //    //  tableQueue.Enqueue(MakeTable());
        //    Console.WriteLine("Table count [{0}]", _purgingTables.Count);
        //    Run(_purgingTables);
        //}

        public void Run()
        {
            var purgingTables = _dataCleanupManager.GetDataPurgingConfiguration().Result;
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            int threadCount = purgingTables.Count;
            Task[] workers = new Task[threadCount];
            for (int i = 0; i < threadCount; ++i)
            {
                int workerId = i;
                Task task = new Task(() => Worker(workerId), token);
                workers[i] = task;
                task.Start();
            }
            foreach (var item in purgingTables)
            {
                _purgingTables.Enqueue(item);
            }
            try
            {
                Task.WhenAll(workers);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Done.");

            Console.ReadLine();
        }

        public void Worker(int workerId)
        {
            Console.WriteLine("Worker {0} is starting.", workerId);

            bool doneEnqueueing = true;
            do
            {
                DataCleanupConfiguration op;
                while (_purgingTables.TryDequeue(out op))
                {
                    Console.WriteLine("Worker {0} is processing item {1}", workerId, op.ColumnName);
                    try
                    {
                        var tries = 0;
                        var maxRetryCount = 5;
                        while (tries < maxRetryCount)
                        {
                            try
                            {
                                tries++;
                                var noOfDeletedData = _dataCleanupManager.DataPurging(op).Result;
                                break;
                            }
                            catch (Exception)
                            {
                                throw;
                                //add log
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        // _log.Info("AddExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                        // _log.Error(ex.ToString());

                    }
                    finally
                    {
                    }
                    // Task.Delay(TimeSpan.FromMilliseconds(1)).Wait();
                    SpinWait.SpinUntil(() => Volatile.Read(ref doneEnqueueing) || (_dataCleanupConfigurations.Count > 0));
                }
                SpinWait.SpinUntil(() => Volatile.Read(ref doneEnqueueing) || (_dataCleanupConfigurations.Count > 0));
            }
            while (!Volatile.Read(ref doneEnqueueing) || (_dataCleanupConfigurations.Count > 0));

            //logic to insert data into logtable
            Console.WriteLine("Worker {0} is stopping.", workerId);
        }
        private async void GetTbalesForConcurrentQueue(ConcurrentQueue<DataCleanupConfiguration> cleanupTables)
        {
            var purgingTables = await _dataCleanupManager.GetDataPurgingConfiguration();

            foreach (var item in purgingTables)
            {
                cleanupTables.Enqueue(item);
            }

        }



    }
}
