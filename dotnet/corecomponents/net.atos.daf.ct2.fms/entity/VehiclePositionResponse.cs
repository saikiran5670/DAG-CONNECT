using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.fms.entity
{
    public class VehiclePositionResponse
    {
        public long RequestTimestamp { get; set; }
        public List<VehiclePosition> VehiclePosition { get; set; }
    }
    public class VehiclePosition
    {
        public string VIN { get; set; }
        public int Altitude { get; set; }
        public int Heading { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long GPSTimestamp { get; set; }
        public int Speed { get; set; }
    }
}


