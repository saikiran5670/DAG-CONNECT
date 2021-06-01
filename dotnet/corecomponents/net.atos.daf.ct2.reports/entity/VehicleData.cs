using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleData
    {
        public string VehicleName { get; set; }
        public string RegNo { get; set; }
        public List<TripDetails> TripData { get; set; }
    }
}
