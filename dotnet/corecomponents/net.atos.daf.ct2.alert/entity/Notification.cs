using System.Collections.Generic;

namespace net.atos.daf.ct2.alert.entity
{
    public class Notification
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        public string AlertUrgencyLevelType { get; set; }
        public string FrequencyType { get; set; }
        public int FrequencyThreshholdValue { get; set; }
        public string ValidityType { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public List<NotificationRecipient> NotificationRecipients { get; set; }
        public List<NotificationLimit> NotificationLimits { get; set; }
        public List<AlertTimingDetail> AlertTimingDetails { get; set; }
    }
}
