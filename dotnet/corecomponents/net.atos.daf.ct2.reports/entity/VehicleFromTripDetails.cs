using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleFromTripDetails
    {
        public string Vin { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
    }
}
