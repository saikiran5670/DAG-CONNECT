using System;
using System.Collections.Generic;
namespace net.atos.daf.ct2.vehicledataservice.Entity
{
    public class VehicleMileageResponse
    {
        public long RequestTimestamp { get; set; }
        public List<Vehicles> Vehicles { get; set; } 
    }

    public class Vehicles
    {
        public string EvtDateTime { get; set; }
        public string VIN { get; set; }
        public decimal TachoMileage { get; set; }
        public decimal GPSMileage { get; set; }
    }
    public class VehiclesCSV
    {
        public string EvtDateTime { get; set; }
        public string VIN { get; set; }
        public decimal TachoMileage { get; set; }        
        public decimal RealMileage { get; set; }
        public string RealMileageAlgorithmVersion { get; set; }
    }
}
