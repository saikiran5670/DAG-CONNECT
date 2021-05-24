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
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverId1 { get; set; }
        public string DriverId2 { get; set; }
        public int Distance { get; set; }
        public string StartAddress { get; set; }
        public string EndAddress { get; set; }
        public double StartPositionlattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double EndPositionLattitude { get; set; }
        public double EndPositionLongitude { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        public List<LiveFleetPosition> LiveFleetPosition { get; set; }
    }
}
