using System.Collections.Generic;

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

        public double AverageSpeed { get; set; }
        public double AverageWeight { get; set; }

        public long Odometer { get; set; }

        public string StartPosition { get; set; }
        public string EndPosition { get; set; }

        public double FuelConsumed { get; set; }
        public long DrivingTime { get; set; }

        public int Alert { get; set; }
        public int Events { get; set; }
        public double FuelConsumed100km { get; set; }

        public double StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double EndPositionLattitude { get; set; }
        public double EndPositionLongitude { get; set; }
        public string RegistrationNo { get; set; }
        public string VehicleName { get; set; }

        public List<LiveFleetPosition> LiveFleetPosition { get; set; }
        public List<TripAlert> TripAlert { get; set; }
    }
    public class TripAlert
    {
        public int Id { get; set; }
        public string TripId { get; set; }
        public int AlertId { get; set; }
        public string AlertName { get; set; }
        public string AlertType { get; set; }
        public long AlertTime { get; set; }
        public string AlertLevel { get; set; }
        public string CategoryType { get; set; }
        public double AlertLatitude { get; set; }
        public double AlertLongitude { get; set; }
        public int AlertGeolocationAddressId { get; set; }
        public string AlertGeolocationAddress { get; set; }
        public long ProcessedMessageTimeStamp { get; set; }
        public string UrgencyLevelType { get; set; }

    }
}
