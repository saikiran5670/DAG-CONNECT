using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetFuel_VehicleGraph
    {
        public int VehicleID { get; set; }
        public string VIN { get; set; }
        public int NumberofTrips { get; set; }
        public double FuelConsumed { get; set; }
        public double Co2Emission { get; set; }
        public double Distance { get; set; }
        public double FuelConsumtion { get; set; }
        public double IdleDuration { get; set; }
        public double Date { get; set; }

    }
}
