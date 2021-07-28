using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class FleetKpiFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
    }
}
