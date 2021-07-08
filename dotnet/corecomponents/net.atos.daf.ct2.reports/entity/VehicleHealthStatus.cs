using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleSummary
    {
        public string VehicleRegNo { get; set; }
        public string VehicleName { get; set; }
        public int Lcts_Id { get; set; }
        public string Lcts_TripId { get; set; }
        public string Lcts_Vin { get; set; }
        public long? Lcts_TripStartTime { get; set; }
        public long? Lcts_TripEndTime { get; set; }
        public string Lcts_Driver1Id { get; set; }
        public int Lcts_TripDistance { get; set; }
        public int Lcts_DrivingTime { get; set; }
        public int Lcts_FuelConsumption { get; set; }
        public string Lcts_VehicleDrivingStatus_type { get; set; }
        public long Lcts_OdometerVal { get; set; }
        public long Lcts_DistanceUntilNextService { get; set; }
        public double Lcts_LatestReceivedPositionLattitude { get; set; }
        public double Lcts_LatestReceivedPositionLongitude { get; set; }
        public double Lcts_LatestReceivedPositionHeading { get; set; }
        public int Lcts_LatestGeolocationAddressId { get; set; }
        public double Lcts_StartPositionLattitude { get; set; }
        public double Lcts_StartPositionLongitude { get; set; }
        public double Lcts_StartPositionHeading { get; set; }
        public int Lcts_StartGeolocationAddressId { get; set; }
        public long? Lcts_LatestProcessedMessageTimestamp { get; set; }
        public string Lcts_VehicleHealthStatusType { get; set; }
        public int Lcts_LatestWrningClass { get; set; }
        public int Lcts_LatestWarningNumber { get; set; }
        public string Lcts_LatestWarningType { get; set; }
        public long? Lcts_LatestWarningTimestamp { get; set; }
        public double Lcts_LatestWarningPositionLatitude { get; set; }
        public double Lcts_LatestWarningPositionLongitude { get; set; }
        public int Lcts_LatestWarningGeolocationAddressId { get; set; }
        public string Lcts_Address { get; set; }
    }
    public class VehicleHealthWarning
    {
        public int WarningId { get; set; }
        public string DriverName { get; set; }

        public string WarningTripId { get; set; }
        public string WarningVin { get; set; }
        public long? WarningTimetamp { get; set; }

        public int WarningClass { get; set; }

        public int WarningNumber { get; set; }

        public double WarningLat { get; set; }

        public double WarningLng { get; set; }

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
    }
    public class VehicleHealthStatus
    {
        public List<VehicleHealthWarning> VehhicleWarnings { get; set; }
        public VehicleSummary VehicleSummary { get; set; }
    }


}
