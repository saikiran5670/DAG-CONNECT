using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class NotificationConfiguration
    {
        public bool IsEmailSend { get; set; } = false;
        public bool IsSMSSend { get; set; } = false;
        public bool IsWebServiceCall { get; set; } = false;
        public int ThreadSleepTimeInSec { get; set; }
        public int EmailRetryCount { get; set; }
        public int SMSRetryCount { get; set; }
        public int WebServiceRetryCount { get; set; }
        public int CancellationTokenDuration { get; set; }
    }
}
