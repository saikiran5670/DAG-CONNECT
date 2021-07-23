
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewDetails
    {
        public int Id { get; set; }
        public string TripId { get; set; }
        public string Vin { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        public string Driver1Id { get; set; }
        public int TripDistance { get; set; }
        public int DrivingTime { get; set; }
        public int FuelConsumption { get; set; }
        public string VehicleDrivingStatusType { get; set; }
        public long OdometerVal { get; set; }
        public long DistanceUntilNextService { get; set; }
        public double LatestReceivedPositionLattitude { get; set; }
        public double LatestReceivedPositionLongitude { get; set; }
        public double LatestReceivedPositionHeading { get; set; }
        public double StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double StartPositionHeading { get; set; }
        public long LatestProcessedMessageTimeStamp { get; set; }
        public string VehicleHealthStatusType { get; set; }
        public int LatestWarningClass { get; set; }
        public int LatestWarningNumber { get; set; }
        public string LatestWarningType { get; set; }
        public long LatestWarningTimestamp { get; set; }
        public double LatestWarningPositionLatitude { get; set; }
        public double LatestWarningPositionLongitude { get; set; }
        public string Vid { get; set; }
        public string RegistrationNo { get; set; }
        // public string DriverFirstName { get; set; }
        // public string DriverLastName { get; set; }
        public string DriverName { get; set; }
        public int LatestGeolocationAddressId { get; set; }
        public string LatestGeolocationAddress { get; set; }
        public int StartGeolocationAddressId { get; set; }
        public string StartGeolocationAddress { get; set; }
        public int LatestWarningGeolocationAddressId { get; set; }
        public string LatestWarningGeolocationAddress { get; set; }
        public string LatestWarningName { get; set; }
        public List<LiveFleetPosition> LiveFleetPositions { get; set; }
        public List<FleetOverviewAlert> FleetOverviewAlert { get; set; }
        public string VehicleName { get; set; }
    }
}
