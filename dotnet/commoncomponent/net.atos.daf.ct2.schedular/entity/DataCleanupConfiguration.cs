namespace net.atos.daf.ct2.schedular.entity
{
    public class DataCleanupConfiguration
    {
        public int Id { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int RetentionPeriod { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }
    public class PurgingConfiguration
    {
        public bool IsWebServiceCall { get; set; }
        public int ThreadSleepTimeInSec { get; set; }
        public int RetryCount { get; set; }
        public int WebServiceRetryCount { get; set; }
        public int CancellationTokenDuration { get; set; }

       

    }
}
