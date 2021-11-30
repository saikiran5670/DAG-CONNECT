//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using net.atos.daf.ct2.schedular;
//using net.atos.daf.ct2.schedular.entity;

//namespace net.atos.daf.ct2.schedularservice.ServiceSchedular
//{
//    public class DataCleanupQueue
//    {
//        private readonly ConcurrentQueue<DataCleanupConfiguration> _sqsQueue;
//        private readonly BlockingCollection<DataCleanupConfiguration> _collection;
//        private readonly ConcurrentBag<String> _result;

//        private readonly IDataCleanupManager _dataCleanupManager;
//        public DataCleanupQueue(IDataCleanupManager dataCleanupManager)
//        {
//            _dataCleanupManager = dataCleanupManager;
//            _sqsQueue = new ConcurrentQueue<DataCleanupConfiguration>();
//            _collection = new BlockingCollection<DataCleanupConfiguration>();
//            _result = new ConcurrentBag<String>();
//        }
//        public async Task CleanupDataFromTables()
//        {
//            var purgingTables = new List<DataCleanupConfiguration>();
//            // Here we separate all the Tasks in distinct threads
//            Task sqs = Task.Run(async () =>
//            {
//                Console.WriteLine("Enequeue on thread " + Thread.CurrentThread.ManagedThreadId.ToString());
//                while (true)
//                {
//                    purgingTables = await _dataCleanupManager.GetDataPurgingConfiguration();
//                    GetTablesToDataPurge(purgingTables);
//                    await Task.Delay(300000); // execute every 5 min =300000 millisec
//                }
//            });
//            Task deq = Task.Run(async () =>
//            {
//                Console.WriteLine("Dequeue on thread " + Thread.CurrentThread.ManagedThreadId.ToString());
//                //  while (true)
//                {
//                    DequeueData(purgingTables);
//                    await Task.Delay(100);
//                }
//            });

//            //Task process = Task.Run(() =>
//            //{
//            //    Console.WriteLine("Process on thread " + Thread.CurrentThread.ManagedThreadId.ToString());
//            //    BackgroundParallelConsumer(); // Process all the Strings in the BlockingCollection
//            //});

//            await Task.WhenAll(sqs, deq);
//        }

//        public void DequeueData(List<DataCleanupConfiguration> purgingTables)
//        {
//            //foreach (var i in purgingTables)
//            {
//                DataCleanupConfiguration dequeued = new DataCleanupConfiguration();
//                if (_sqsQueue.TryDequeue(out dequeued))
//                {
//                    var tries = 0;
//                    var maxRetryCount = 5;
//                    while (tries < maxRetryCount)
//                    {
//                        try
//                        {
//                            tries++;
//                            var data = _dataCleanupManager.DataPurging(dequeued).Result;
//                            Console.WriteLine("Dequeued : " + dequeued); break;
//                        }
//                        catch (Exception)
//                        {
//                            // if (tries > 3)

//                            // StopAsync(new CancellationToken()).ConfigureAwait(true); ;
//                            throw;
//                            //add log
//                        }
//                    }
//                }
//            }
//        }

//        public void GetTablesToDataPurge(List<DataCleanupConfiguration> purgingTables)
//        {

//            Console.WriteLine(" ---------- Enaqueue tables  ---------- ");
//            foreach (var data in purgingTables)//Enumerable.Range(0, 50).Select(i => Path.GetRandomFileName().Split('.').FirstOrDefault()))
//                _sqsQueue.Enqueue(data);
//        }

//        //public void BackgroundParallelConsumer()
//        //{
//        //    // Here we stay in Parallel.ForEach, waiting for data. Once processed, we are still waiting the next chunks
//        //    Parallel.ForEach(_collection.GetConsumingEnumerable(), (i) =>
//        //    {
//        //        // Processing Logic
//        //        String processedData = "Processed : " + i;
//        //        _result.Add(processedData);
//        //        Console.WriteLine(processedData);
//        //    });

//        //}
//    }
//}

