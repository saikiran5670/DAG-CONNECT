using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewResult
    {
        public int Lcts_Id { get; set; }
        public String Lcts_TripId { get; set; }
        public String Lcts_Vin { get; set; }
        public long Lcts_StartTimeStamp { get; set; }
        public long Lcts_EndTimeStamp { get; set; }
        public String Lcts_Driver1Id { get; set; }
        public String Lcts_TripDistance { get; set; }
        public String Lcts_DrivingTime { get; set; }
        public int Lcts_FuelConsumption { get; set; }
        public string Lcts_VehicleDrivingStatusType { get; set; }
        public int Lcts_OdometerVal { get; set; }
        public long Lcts_DistanceUntilNextService { get; set; }
        public long Lcts_LatestReceivedPositionLattitude { get; set; }
        public double Lcts_LatestReceivedPositionLongitude { get; set; }
        public double Lcts_LatestReceivedPositionHeading { get; set; }
        public int Lcts_StartPositionLattitude { get; set; }
        public double Lcts_StartPositionLongitude { get; set; }
        public double Lcts_StartPositionHeading { get; set; }
        public int Lcts_LatestProcessedMessageTimeStamp { get; set; }
        public long Lcts_VehicleHealthStatusType { get; set; }
        public int Lcts_LatestWarningClass { get; set; }
        public int Lcts_LatestWarningNumber { get; set; }
        public String Lcts_LatestWarningType { get; set; }
        public long Lcts_LatestWarningTimestamp { get; set; }
        public double Lcts_LatestWarningPositionLatitude { get; set; }
        public double Lcts_LatestWarningPositionLongitude { get; set; }
        public String Veh_Vid { get; set; }
        public String Veh_RegistrationNo { get; set; }
        public String Dri_FirstName { get; set; }
        public String Dri_LastName { get; set; }
        public int Geoadd_Id { get; set; }
        public String Geoadd_Address { get; set; }
        public int Lps_Id { get; set; }
        public String Lps_TripId { get; set; }
        public double Lps_GpsAltitude { get; set; }
        public double Lps_GpsHeading { get; set; }
        public double Lps_GpsLatitude { get; set; }
        public double Lps_GpsLongitude { get; set; }
        public double Lps_Co2Emission { get; set; }
        public double Lps_FuelConsumption { get; set; }
        public int Lps_LastOdometerVal { get; set; }
        public int Latgeoadd_LatestGeolocationAddressId { get; set; }
        public string Latgeoadd_LatestGeolocationAddress { get; set; }
        public int Stageoadd_StartGeolocationAddressId { get; set; }
        public string Stageoadd_StartGeolocationAddress { get; set; }
        public int Wangeoadd_LatestWarningGeolocationAddressId { get; set; }
        public string Wangeoadd_LatestWarningGeolocationAddress { get; set; }
    }
}
