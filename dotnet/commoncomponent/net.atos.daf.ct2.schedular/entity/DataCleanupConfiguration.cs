namespace net.atos.daf.ct2.schedular.entity
{
    public class DataCleanupConfiguration
    {
        public bool IsSMSSend { get; set; } = false;
        public bool IsWebServiceCall { get; set; } = false;
        public int ThreadSleepTimeInSec { get; set; }
        public int RetentionCount { get; set; }
        public int WebServiceRetryCount { get; set; } = 5;
        public int CancellationTokenDuration { get; set; }
    }
}
