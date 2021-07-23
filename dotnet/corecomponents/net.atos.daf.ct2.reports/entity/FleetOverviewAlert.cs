using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewAlert
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        public string AlertName { get; set; }
        public string AlertType { get; set; }
        public long AlertTime { get; set; }
        public string AlertLevel { get; set; }
        public string CategoryType { get; set; }
        public double AlertLatitude { get; set; }
        public double AlertLongitude { get; set; }
        public int AlertGeolocationAddressId { get; set; }
        public string AlertGeolocationAddress { get; set; }
    }
}
