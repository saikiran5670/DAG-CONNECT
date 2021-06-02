using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class DriverActivityFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public string[] VIN { get; set; }
        public int[] DriverId { get; set; }
    }
}
