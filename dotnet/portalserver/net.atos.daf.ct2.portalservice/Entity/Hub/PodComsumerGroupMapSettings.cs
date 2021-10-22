using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Hub
{
    public class PodConsumerGroupMapSettings
    {
        public Dictionary<string, string> PodConsumerGroupMap
        {
            get;
            set;
        }
    }

    public static class NotificationHubConstant
    {
        public const string ALERT_FEATURE_STARTWITH = "Alerts.";
    }
}
