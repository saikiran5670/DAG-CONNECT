using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetUtilizationFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VIN { get; set; }
    }
}
