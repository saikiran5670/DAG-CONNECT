using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class FleetOverviewDetails
    {
        public int Id { get; set; }
        public string TripId { get; set; }
        public string Vin { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        public string Driver1Id { get; set; }
        public string TripDistance { get; set; }
        public string DrivingTime { get; set; }
        public int FuelConsumption { get; set; }
        public int VehicleDrivingStatusType { get; set; }
        public int OdometerVal { get; set; }
        public long DistanceUntilNextService { get; set; }
        public long LatestReceivedPositionLattitude { get; set; }
        public double LatestReceivedPositionLongitude { get; set; }
        public double LatestReceivedPositionHeading { get; set; }
        public int StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double StartPositionHeading { get; set; }
        public int LatestProcessedMessageTimeStamp { get; set; }
        public long VehicleHealthStatusType { get; set; }
        public int LatestWarningClass { get; set; }
        public int LatestWarningNumber { get; set; }
        public string LatestWarningType { get; set; }
        public long LatestWarningTimestamp { get; set; }
        public double LatestWarningPositionLatitude { get; set; }
        public double LatestWarningPositionLongitude { get; set; }
        public string Vid { get; set; }
        public string RegistrationNo { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public int LatestGeolocationAddressId { get; set; }
        public string LatestGeolocationAddress { get; set; }
        public int StartGeolocationAddressId { get; set; }
        public string StartGeolocationAddress { get; set; }
        public int LatestWarningGeolocationAddressId { get; set; }
        public string LatestWarningGeolocationAddress { get; set; }
        public List<LiveFleetPosition> LiveFleetPositions { get; set; }
    }
}
