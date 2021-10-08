using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.fmsdataservice.entity
{
    public class VehiclePosition
    {
        public string VIN { get; set; }
        public int Altitude { get; set; }
        public int Heading { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int GPSTimestamp { get; set; }
        public int Speed { get; set; }
    }

    public class VehiclePositionResponse
    {
        public int RequestTimestamp { get; set; }
        public List<VehiclePosition> VehiclePosition { get; set; }
    }
    public class VehiclePositionRequest
    {
        public string VIN { get; set; }
        public string Since { get; set; }
    }
}
