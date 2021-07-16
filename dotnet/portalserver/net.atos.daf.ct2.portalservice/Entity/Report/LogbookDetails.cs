using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class LogbookDetails
    {

        public string VIN { get; set; }
        public string GroupId { get; set; }
        public string TripId { get; set; }
        public string VehicleRegNo { get; set; }
        public string VehicleName { get; set; }
        public string AlertName { get; set; }
        public string AlertType { get; set; }
        public int Occurrence { get; set; }
        public char AlertLevel { get; set; }
        public char AlertCategory { get; set; }
        public long TripStartTime { get; set; }
        public long TripEndTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Threshold { get; set; }
    }
}
