using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewDetails
    {
        public int Id { get; set; }
        public String TripId { get; set; }
        public String Vin { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        public String Driver1Id { get; set; }
        public String TripDistance { get; set; }
        public String DrivingTime { get; set; }
        public int FuelConsumption { get; set; }
        public int VehicleDrivingStatusType { get; set; }
        public int OdometerVal { get; set; }
        public long DistanceUntilNextService { get; set; }
        public long LatestReceivedPositionLattitude { get; set; }
        public double LatestReceivedPositionLongitude { get; set; }
        public double LatestReceivedPositionHeading { get; set; }
        public double LatestGeolocationAddressId { get; set; }
        public int StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double StartPositionHeading { get; set; }
        public double StartGeolocationAddressId { get; set; }
        public int LatestProcessedMessageTimeStamp { get; set; }
        public long VehicleHealthStatusType { get; set; }
        public int LatestWarningClass { get; set; }
        public int LatestWarningNumber { get; set; }
        public String LatestWarningType { get; set; }
        public long LatestWarningTimestamp { get; set; }
        public double LatestWarningPositionLatitude { get; set; }
        public double LatestWarningPositionLongitude { get; set; }
        public int LatestWarningGeolocationAddressId { get; set; }
        public String Vid { get; set; }
        public String RegistrationNo { get; set; }
        public String DriverFirstName { get; set; }
        public String DriverLastName { get; set; }
        public List<LiveFleetPosition> LiveFleetPositions { get; set; }
    }
}
