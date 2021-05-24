using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class TripDetails
    {
        public int Id { get; set; }
        public string TripId { get; set; }
        public string VIN { get; set; }
        
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        
        public int Distance { get; set; }
        public int IdleDuration { get; set; }
        
        public int AverageSpeed { get; set; }
        public int AverageWeight { get; set; }
        
        public long Odometer { get; set; }
        
        public string StartPosition { get; set; }
        public string EndPosition { get; set; }

        public double FuelConsumed { get; set; }
        public int DrivingTime { get; set; }

        public int Alert { get; set; }
        public int Events { get; set; }
        public double FuelConsumed100km { get; set; }

        public double StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double EndPositionLattitude { get; set; }
        public double EndPositionLongitude { get; set; }

        public List<LiveFleetPosition> LiveFleetPosition { get; set; }
    }
}
