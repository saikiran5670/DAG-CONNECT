using System;
using System.Collections.Generic;
using System.Text;
using net.atos.daf.ct2.reportscheduler.ENUM;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportEmailFrequency
    {
        public int ReportId { get; set; }
        public long ReportScheduleRunDate { get; set; }
        public long ReportNextScheduleRunDate { get; set; }
        public long ReportPrevioudScheduleRunDate { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public TimeFrequenyType FrequencyType { get; set; }
    }
}
