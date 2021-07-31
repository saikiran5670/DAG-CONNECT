using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class FleetKpi
    {
        public int VehicleCount { get; set; }
        public double Co2Emission { get; set; }
        public double IdlingTime { get; set; }
        public double DrivingTime { get; set; }
        public double Distance { get; set; }
        public double FuelConsumption { get; set; }
        public double Idlingfuelconsumption { get; set; }
        public FleetKpi LastChangeKpi { get; set; }
    }
}
