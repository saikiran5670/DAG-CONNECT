﻿using System;
using System.Collections.Generic;
using System.Text;

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
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public List<NotificationRecipient> NotificationRecipients { get; set; }
        public List<NotificationLimit> NotificationLimits { get; set; }
        public List<NotificationAvailabilityPeriod> NotificationAvailabilityPeriods { get; set; }
    }
}
