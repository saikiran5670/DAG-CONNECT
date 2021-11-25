using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.schedular.entity;

namespace net.atos.daf.ct2.schedular
{
    public class DataCleanup
    {


        private readonly DataCleanupManager _dataCleanupManager;
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private readonly ConcurrentQueue<DataCleanupConfiguration> _dataCleanupConfigurations;
        public DataCleanup(DataCleanupManager dataCleanupManager, IDataAccess dataAccess, IDataMartDataAccess dataMartDataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartDataAccess = dataMartDataAccess;
            _dataCleanupManager = dataCleanupManager;
            _dataCleanupConfigurations = new ConcurrentQueue<DataCleanupConfiguration>();
        }



        public void Run()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            int threadCount = 4;
            Task[] workers = new Task[threadCount];

            for (int i = 0; i < threadCount; ++i)
            {
                int workerId = i;
                Task task = new Task(() => Worker(workerId), token);
                workers[i] = task;
                task.Start();
            }
            var purgingTables = //_dataCleanupManager.GetDataPurgingConfiguration().Result;
            new List<DataCleanupConfiguration>() {
              new DataCleanupConfiguration() {  ColumnName="aaa",  DatabaseName="111"  },
              new DataCleanupConfiguration() {  ColumnName="sdsdsd",  DatabaseName="333333"  },
              new DataCleanupConfiguration() {  ColumnName="nnnnnnnnnnn",  DatabaseName="4444444"  },
        };

            foreach (var item in purgingTables)
            {
                _dataCleanupConfigurations.Enqueue(item);
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

        void Worker(int workerId)
        {
            Console.WriteLine("Worker {0} is starting.", workerId);

            bool doneEnqueueing = true;
            do
            {
                DataCleanupConfiguration op;
                while (_dataCleanupConfigurations.TryDequeue(out op))
                {
                    Console.WriteLine("Worker {0} is processing item {1}", workerId, op.ColumnName);
                    _dataAccess.Connection.Open();
                    var transactionScope = _dataAccess.Connection.BeginTransaction();

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

                        transactionScope.Commit();

                    }
                    catch (Exception ex)
                    {
                        transactionScope.Rollback();
                        // _log.Info("AddExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                        // _log.Error(ex.ToString());

                    }
                    finally
                    {
                        _dataAccess.Connection.Close();
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

    }

}
