using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertUrgencyLevelRefBase
    {
        public string UrgencyLevelType { get; set; }

        public double ThresholdValue { get; set; }

        public string UnitType { get; set; }

        public bool[] DayType { get; set; } = new bool[7];

        public string PeriodType { get; set; }

        public long UrgencylevelStartDate { get; set; }

        public long UrgencylevelEndDate { get; set; }

    }
    public class AlertUrgencyLevelRef :AlertUrgencyLevelRefBase
    {
        public List<AlertFilterRef> AlertFilterRefs { get; set; } = new List<AlertFilterRef>();
    }
    public class AlertUrgencyLevelRefEdit: AlertUrgencyLevelRefBase
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        //public string State { get; set; }
        //public long ModifiedAt { get; set; }
        public List<AlertFilterRefEdit> AlertFilterRefs { get; set; } = new List<AlertFilterRefEdit>();
    }
}
