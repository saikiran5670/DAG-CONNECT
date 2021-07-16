using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class DriverActivityChart
    {
        public long ActivityDate { get; set; }
        public int Code { get; set; }
        public long Duration { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
    }
}
