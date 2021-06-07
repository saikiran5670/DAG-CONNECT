using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class AlertTimingDetail
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int RefId { get; set; }
        public bool[] DayType { get; set; } = new bool[7];
        public string PeriodType { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }
}
