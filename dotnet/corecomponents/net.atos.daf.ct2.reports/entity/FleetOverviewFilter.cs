using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewFilter
    {
        public string GroupId { get; set; }
        public string AlertLevel { get; set; }
        public string AlertCategory { get; set; }
        public string HealthStatus { get; set; }
        public string OtherFilter { get; set; }
        public string DriverId { get; set; }
        public string Days { get; set; }
    }

}
