﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class Alert
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public string ValidityPeriodType { get; set; }

        public long ValidityStartDate { get; set; }

        public long ValidityEndDate { get; set; }

        public int VehicleGroupId { get; set; }

        public string State { get; set; }

        public long CreatedAt { get; set; }

        public int CreatedBy{ get; set; }

        public long ModifiedAt { get; set; }

        public int ModifiedBy { get; set; }

        public List<AlertUrgencyLevelRef> AlertUrgencyLevelRefs { get; set; } = new List<AlertUrgencyLevelRef>();

        public List<AlertFilterRef> AlertFilterRefs { get; set; } = new List<AlertFilterRef>();

        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public List<AlertLandmarkRef> AlertLandmarkRefs { get; set; } = new List<AlertLandmarkRef>();
    }
}
