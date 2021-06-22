using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class Calender_Fleetutilization
    {
        public long CalenderDate { get; set; }
        public double Averagedistance { get; set; }
        public double Averagetriptime { get; set; }
        public double Averagedrivingtime { get; set; }
        public int Averageidleduration { get; set; }
        public int Averagedistanceperday { get; set; }
        public int AverageSpeed { get; set; }
        public int Averageweightperprip { get; set; }
        public string VIN { get; set; }
        public int Vehiclecount { get; set; }
        public int Tripcount { get; set; }

    }
}
