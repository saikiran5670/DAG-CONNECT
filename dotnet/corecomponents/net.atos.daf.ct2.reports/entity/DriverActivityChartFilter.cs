using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class DriverActivityChartFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public string DriverId { get; set; }
    }
}
