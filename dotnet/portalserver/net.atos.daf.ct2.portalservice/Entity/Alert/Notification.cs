using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class NotificationBase
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
    }
    public class Notification: NotificationBase
    {
        //public int Id { get; set; }
        //public int AlertId { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
        //public int ModifiedBy { get; set; }
        public List<NotificationRecipient> NotificationRecipients { get; set; }
        public List<NotificationLimit> NotificationLimits { get; set; }
        public List<NotificationAvailabilityPeriod> NotificationAvailabilityPeriods { get; set; }
    }
    public class NotificationEdit: NotificationBase
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public List<NotificationRecipientEdit> NotificationRecipients { get; set; }
        public List<NotificationLimitEdit> NotificationLimits { get; set; }
        public List<NotificationAvailabilityPeriodEdit> NotificationAvailabilityPeriods { get; set; }
    }
}
