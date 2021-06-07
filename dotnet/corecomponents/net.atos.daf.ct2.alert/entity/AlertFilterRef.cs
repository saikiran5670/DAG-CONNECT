using System.Collections.Generic;

namespace net.atos.daf.ct2.alert.entity
{
    public class AlertFilterRef
    {
        public int Id { get; set; }

        public int AlertId { get; set; }

        public int AlertUrgencyLevelId { get; set; }

        public string FilterType { get; set; }

        public double ThresholdValue { get; set; }

        public string UnitType { get; set; }

        public string LandmarkType { get; set; }

        public int RefId { get; set; }

        public string PositionType { get; set; }

        //public bool[] DayType { get; set; } = new bool[7];

        //public string PeriodType { get; set; }

        //public long FilterStartDate { get; set; }

        //public long FilterEndDate { get; set; }

        public string State { get; set; }

        public long CreatedAt { get; set; }

        public long ModifiedAt { get; set; }
        public List<AlertTimingDetail> AlertTimingDetails { get; set; }
    }
}
