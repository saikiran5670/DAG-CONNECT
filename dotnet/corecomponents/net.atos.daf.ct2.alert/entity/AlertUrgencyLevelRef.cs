using System.Collections.Generic;

namespace net.atos.daf.ct2.alert.entity
{
    public class AlertUrgencyLevelRef
    {
        public int Id { get; set; }

        public int AlertId { get; set; }

        public string UrgencyLevelType { get; set; }

        public double ThresholdValue { get; set; }

        public string UnitType { get; set; }

        public bool[] DayType { get; set; } = new bool[7];

        public string PeriodType { get; set; }

        public long UrgencylevelStartDate { get; set; }

        public long UrgencylevelEndDate { get; set; }

        public string State { get; set; }

        public long CreatedAt { get; set; }

        public long ModifiedAt { get; set; }

        public List<AlertFilterRef> AlertFilterRefs { get; set; } = new List<AlertFilterRef>();
        public List<AlertTimingDetail> AlertTimingDetails { get; set; }

    }
}
