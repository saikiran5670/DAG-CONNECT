using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class Chart_Fleetutilization
    {
        public long CalenderDate { get; set; }
        public double Distance { get; set; }
        public double Triptime { get; set; }
        public double Drivingtime { get; set; }
        public double Idleduration { get; set; }
        public double Distanceperday { get; set; }
        public double Speed { get; set; }
        public double Weight { get; set; }
        public double Fuelconsumption { get; set; }
        public double Fuelconsumed { get; set; }
        public string VIN { get; set; }
        public int Vehiclecount { get; set; }
        public int Tripcount { get; set; }

    }
}
