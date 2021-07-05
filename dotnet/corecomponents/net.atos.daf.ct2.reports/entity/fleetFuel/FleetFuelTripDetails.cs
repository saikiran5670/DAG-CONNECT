using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity.fleetFuel
{
   public class FleetFuelTripDetails : FleetFuelDetails
    {    
        public double StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double EndPositionLattitude { get; set; }
        public double EndPositionLongitude { get; set; }     
        public string StartPosition { get; set; }
        public string EndPosition { get; set; }
    }
}
