using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportCreationScheduler : ReportScheduler
    {
        public string ReportName { get; set; }

        public string ReportKey { get; set; }

        public int TimeZoneId { get; set; }
    }
}
