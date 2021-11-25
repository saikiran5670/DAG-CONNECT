//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using net.atos.daf.ct2.schedular;
//using net.atos.daf.ct2.schedular.entity;

//namespace net.atos.daf.ct2.schedularservice.ServiceSchedular
//{
//    public class DataCleanup
//    {

//        private readonly DataCleanupQueue _tableQueue = new DataCleanupQueue();
//        private readonly Func<DataCleanupConfiguration, String> _runPostProcess;

//        private readonly DataCleanupManager _dataCleanupManager;
//        public DataCleanup(DataCleanupManager dataCleanupManager)
//        {
//            _runPostProcess = new Func<DataCleanupConfiguration, String>(SerializeTable);
//            _tableQueue.TableQueued += new EventHandler<TableQueuedEventArgs>(TableQueue_TableQueued);
//            _dataCleanupManager = dataCleanupManager;
//        }

//        void TableQueue_TableQueued(object sender, TableQueuedEventArgs e)
//        {
//            //  do something with table
//            //  I can't figure out is how to pass custom object in 3rd parameter
//            _runPostProcess.BeginInvoke(e.Table, new AsyncCallback(PostComplete), e.Table.Id);
//        }

//        public void ExtractData()
//        {

//            var purgingTables = _dataCleanupManager.GetDataPurgingConfiguration().Result;

//            foreach (var item in purgingTables)
//            {
//                _tableQueue.Enqueue(item);
//            }
//            // perform data extraction
//            //  tableQueue.Enqueue(MakeTable());
//            Console.WriteLine("Table count [{0}]", _tableQueue.Count);
//        }

//        //private DataCleanupConfiguration MakeTable()
//        //{ 
//        //  return new DataCleanupConfiguration(String.Format("Table{0}", _indexer++)); }

//        private string SerializeTable(DataCleanupConfiguration Table)
//        {
//            _dataAccess.Connection.Open();
//            var transactionScope = _dataAccess.Connection.BeginTransaction();


//            string file = Table.TableName + ".xml";


//            // filename = file;

//            return file;
//        }

//        private void PostComplete(IAsyncResult iasResult)
//        {
//            string file = (string)iasResult.AsyncState;
//            Console.WriteLine("[{0}]Completed: {1}", Thread.CurrentThread.ManagedThreadId, file);

//            _runPostProcess.EndInvoke(iasResult);
//        }


//    }

//    public sealed class DataCleanupQueue : ConcurrentQueue<DataCleanupConfiguration>
//    {
//        public event EventHandler<TableQueuedEventArgs> TableQueued;

//        public DataCleanupQueue()
//        { }
//        public DataCleanupQueue(IEnumerable<DataCleanupConfiguration> TableCollection)
//            : base(TableCollection)
//        { }

//        new public void Enqueue(DataCleanupConfiguration Table)
//        {
//            base.Enqueue(Table);
//            OnTableQueued(new TableQueuedEventArgs(Table));
//        }

//        public void OnTableQueued(TableQueuedEventArgs table)
//        {
//            EventHandler<TableQueuedEventArgs> handler = TableQueued;

//            if (handler != null)
//            {
//                handler(this, table);
//            }
//        }
//    }

//    public class TableQueuedEventArgs : EventArgs
//    {

//        public TableQueuedEventArgs(DataCleanupConfiguration Table)
//        {
//            this.Table = Table;
//        }



//        public DataCleanupConfiguration Table { get; set; }

//    }
//}
