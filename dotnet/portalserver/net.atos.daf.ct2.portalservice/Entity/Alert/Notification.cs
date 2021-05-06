using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class Notification
    {
        //public int Id { get; set; }
        //public int AlertId { get; set; }
        public string AlertUrgencyLevelType { get; set; }
        public string FrequencyType { get; set; }
        public int FrequencyThreshholdValue { get; set; }
        public string ValidityType { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        //public long ModifiedAt { get; set; }
        //public int ModifiedBy { get; set; }
        public List<NotificationRecipient> NotificationRecipients { get; set; }
        public List<NotificationLimit> NotificationLimits { get; set; }
        public List<NotificationAvailabilityPeriod> NotificationAvailabilityPeriods { get; set; }
    }
    public class NotificationEdit: Notification
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public new List<NotificationRecipientEdit> NotificationRecipients { get; set; }
        public new List<NotificationLimitEdit> NotificationLimits { get; set; }
        public new List<NotificationAvailabilityPeriodEdit> NotificationAvailabilityPeriods { get; set; }
    }
}
