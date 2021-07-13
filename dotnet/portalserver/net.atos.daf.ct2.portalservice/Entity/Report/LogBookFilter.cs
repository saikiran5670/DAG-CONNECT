using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class LogBookFilter
    {
        public List<VehicleGroup> VehicleGroups { get; set; }
        public List<FilterProperty> AlertLevel { get; set; }
        public List<FilterProperty> AlertCategory { get; set; }
        public List<FilterProperty> AlertType { get; set; }
    }
}
