using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class VehicleHealthStatusRequest
    {
        public string VIN { get; set; }
        public string TripId { get; set; } = string.Empty;
        public int Days { get; set; }
        public string WarningType { get; set; } = string.Empty;
        public string LngCode { get; set; } = string.Empty;
    }
}
