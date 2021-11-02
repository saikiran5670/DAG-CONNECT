using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class TripFilterRequest
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public string VIN { get; set; }
        public List<int> FeatureIds { get; set; }
        public string AlertVIN { get; set; }
    }
}
