using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class NotificationViewHistory
    {
        public string TripId { get; set; }
        public string Vin { get; set; }
        public string AlertCategory { get; set; }
        public string AlertType { get; set; }
        public long AlertGeneratedTime { get; set; }
        public int OrganizationId { get; set; }
        public int TripAlertId { get; set; }
        public int AlertId { get; set; }
        public int AccountId { get; set; }
        public long AlertViewTimestamp { get; set; }
    }
}
