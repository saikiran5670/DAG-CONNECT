using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.vehicle.response
{
    public class Vehicles
    {
        public string VIN { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
    }

    public class VehicleNamelistResponse
    {
        public long RequestTimestamp { get; set; }
        public List<Vehicles> Vehicles { get; set; }
    }
}
