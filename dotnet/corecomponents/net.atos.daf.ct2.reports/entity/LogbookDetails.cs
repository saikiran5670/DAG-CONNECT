using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class LogbookDetails
    {
        public string VIN { get; set; }
        public string GroupId { get; set; }
        public string TripId { get; set; }
        public int AlertId { get; set; }
        public string VehicleRegNo { get; set; }
        public string VehicleName { get; set; }
        public string AlertName { get; set; }
        public string AlertType { get; set; }
        public int Occurrence { get; set; }
        public string AlertLevel { get; set; }
        public string AlertCategory { get; set; }
        public long TripStartTime { get; set; }
        public long TripEndTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AlertGeolocationAddress { get; set; }
        public int ThresholdValue { get; set; }
        public string ThresholdUnit { get; set; }
        public long AlertGeneratedTime { get; set; }
        public int AlertGeolocationAddressId { get; set; }
        public long ProcessedMessageTimestamp { get; set; }



    }
}
