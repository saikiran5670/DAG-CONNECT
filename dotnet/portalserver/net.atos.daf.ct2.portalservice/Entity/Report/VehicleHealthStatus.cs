using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    //public class VehicleSummary
    //{
    //    public string VehicleRegNo { get; set; }
    //    public string VehicleName { get; set; }
    //    public string TripId { get; set; }
    //    public string Vin { get; set; }
    //    public long? TripStartTime { get; set; }
    //    public long? TripEndTime { get; set; }
    //    public string Driver1Id { get; set; }
    //    public int TripDistance { get; set; }
    //    public int DrivingTime { get; set; }
    //    public int FuelConsumption { get; set; }
    //    public string VehicleDrivingStatus_type { get; set; }
    //    public long OdometerVal { get; set; }
    //    public long DistanceUntilNextService { get; set; }
    //    public double LatestReceivedPositionLattitude { get; set; }
    //    public double LatestReceivedPositionLongitude { get; set; }
    //    public double LatestReceivedPositionHeading { get; set; }
    //    public int LatestGeolocationAddressId { get; set; }
    //    public double StartPositionLattitude { get; set; }
    //    public double StartPositionLongitude { get; set; }
    //    public double StartPositionHeading { get; set; }
    //    public int StartGeolocationAddressId { get; set; }
    //    public long? LatestProcessedMessageTimestamp { get; set; }
    //    public string VehicleHealthStatusType { get; set; }
    //    public int LatestWrningClass { get; set; }
    //    public int LatestWarningNumber { get; set; }
    //    public string LatestWarningType { get; set; }
    //    public long? LatestWarningTimestamp { get; set; }
    //    public double LatestWarningPositionLatitude { get; set; }
    //    public double LatestWarningPositionLongitude { get; set; }
    //    public int LatestWarningGeolocationAddressId { get; set; }
    //    public string Address { get; set; }
    //}
    //public class VehicleHealthWarning
    //{
    //    public string VehicleRegNo { get; set; }
    //    public string VehicleName { get; set; }
    //    public string TripId { get; set; }
    //    public string Vin { get; set; }
    //    public long? TripStartTime { get; set; }
    //    public long? TripEndTime { get; set; }
    //    public string Driver1Id { get; set; }
    //    public int TripDistance { get; set; }
    //    public int DrivingTime { get; set; }
    //    public int FuelConsumption { get; set; }
    //    public string VehicleDrivingStatus_type { get; set; }
    //    public long OdometerVal { get; set; }
    //    public long DistanceUntilNextService { get; set; }
    //    public double LatestReceivedPositionLattitude { get; set; }
    //    public double LatestReceivedPositionLongitude { get; set; }
    //    public double LatestReceivedPositionHeading { get; set; }
    //    public int LatestGeolocationAddressId { get; set; }
    //    public double StartPositionLattitude { get; set; }
    //    public double StartPositionLongitude { get; set; }
    //    public double StartPositionHeading { get; set; }
    //    public int StartGeolocationAddressId { get; set; }
    //    public long? LatestProcessedMessageTimestamp { get; set; }
    //    public string VehicleHealthStatusType { get; set; }
    //    public int LatestWrningClass { get; set; }
    //    public int LatestWarningNumber { get; set; }
    //    public string LatestWarningType { get; set; }
    //    public long? LatestWarningTimestamp { get; set; }
    //    public double LatestWarningPositionLatitude { get; set; }
    //    public double LatestWarningPositionLongitude { get; set; }
    //    public int LatestWarningGeolocationAddressId { get; set; }
    //    public string Address { get; set; }
    //    public int WarningId { get; set; }
    //    public string DriverName { get; set; }

    //    public string WarningTripId { get; set; }
    //    public string WarningVin { get; set; }
    //    public long? WarningTimetamp { get; set; }

    //    public int WarningClass { get; set; }

    //    public int WarningNumber { get; set; }

    //    public double WarningLat { get; set; }

    //    public double WarningLng { get; set; }

    //    public double WarningHeading { get; set; }

    //    public string WarningVehicleHealthStatusType { get; set; }

    //    public string WarningVehicleDrivingStatusType { get; set; }

    //    public string WarningDrivingId { get; set; }

    //    public string WarningType { get; set; }
    //    public long WarningDistanceUntilNectService { get; set; }
    //    public long WarningOdometerVal { get; set; }
    //    public long? WarningLatestProcessedMessageTimestamp { get; set; }

    //    public string WarningName { get; set; } //from dtcwarning table
    //    public string WarningAdvice { get; set; }
    //}
    public class VehicleHealthStatus
    {

        public string VehicleRegNo { get; set; }
        public string VehicleName { get; set; }
        public string TripId { get; set; }
        public string Vin { get; set; }
        public long? TripStartTime { get; set; }
        public long? TripEndTime { get; set; }
        public string Driver1Id { get; set; }
        public int TripDistance { get; set; }
        public int DrivingTime { get; set; }
        public int FuelConsumption { get; set; }
        public string VehicleDrivingStatus_type { get; set; }
        public long OdometerVal { get; set; }
        public long DistanceUntilNextService { get; set; }
        public double LatestReceivedPositionLattitude { get; set; }
        public double LatestReceivedPositionLongitude { get; set; }
        public double LatestReceivedPositionHeading { get; set; }
        public int LatestGeolocationAddressId { get; set; }
        public string LatestGeolocationAddress { get; set; }
        public double StartPositionLattitude { get; set; }
        public double StartPositionLongitude { get; set; }
        public double StartPositionHeading { get; set; }
        public string StartGeolocationAddress { get; set; }
        public int StartGeolocationAddressId { get; set; }
        public long? LatestProcessedMessageTimestamp { get; set; }
        public string VehicleHealthStatusType { get; set; }
        public int LatestWrningClass { get; set; }
        public int LatestWarningNumber { get; set; }
        public string LatestWarningType { get; set; }
        public long? LatestWarningTimestamp { get; set; }
        public double LatestWarningPositionLatitude { get; set; }
        public double LatestWarningPositionLongitude { get; set; }
        public int LatestWarningGeolocationAddressId { get; set; }
        public string LatestWarningGeolocationAddress { get; set; }

        public string Address { get; set; }

        public int WarningId { get; set; }
        public string DriverName { get; set; }

        public string WarningTripId { get; set; }
        public string WarningVin { get; set; }
        public long? WarningTimetamp { get; set; }

        public int WarningClass { get; set; }

        public int WarningNumber { get; set; }

        public double WarningLat { get; set; }

        public double WarningLng { get; set; }
        public string WarningAddress { get; set; }

        public double WarningHeading { get; set; }

        public string WarningVehicleHealthStatusType { get; set; }

        public string WarningVehicleDrivingStatusType { get; set; }

        public string WarningDrivingId { get; set; }

        public string WarningType { get; set; }
        public long WarningDistanceUntilNectService { get; set; }
        public long WarningOdometerVal { get; set; }
        public long? WarningLatestProcessedMessageTimestamp { get; set; }

        public string WarningName { get; set; } //from dtcwarning table
        public string WarningAdvice { get; set; }
        // public List<VehicleHealthWarning> VehhicleWarnings { get; set; }
        // public VehicleSummary VehicleSummary { get; set; }
    }
}
