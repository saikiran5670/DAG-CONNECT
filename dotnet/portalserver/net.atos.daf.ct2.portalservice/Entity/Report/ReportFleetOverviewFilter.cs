using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class ReportFleetOverviewFilter
    {
        public List<VehicleGroup> VehicleGroups { get; set; }
        public List<FilterProperty> AlertLevel { get; set; }
        public List<FilterProperty> AlertCategory { get; set; }
        public List<FilterProperty> HealthStatus { get; set; }
        public List<FilterProperty> OtherFilter { get; set; }
        public List<POIResponse> UserPois { get; set; }
        public List<POIResponse> GlobalPois { get; set; }
        
    }

    public class VehicleGroup
    {
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureKey { get; set; }
        public bool Subscribe { get; set; }
    }

    public class FilterProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
