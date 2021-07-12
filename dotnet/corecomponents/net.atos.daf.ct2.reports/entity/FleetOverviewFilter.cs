using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewFilter
    {
        public List<string> GroupId { get; set; }
        public List<string> VINIds { get; set; }
        public List<string> AlertLevel { get; set; }
        public List<string> AlertCategory { get; set; }
        public List<string> HealthStatus { get; set; }
        public List<string> OtherFilter { get; set; }
        public List<string> DriverId { get; set; }
        public int Days { get; set; }
        public string LanguageCode { get; set; }
    }

}
