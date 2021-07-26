namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleHealthResult
    {
        public string VehicleRegNo { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        //public int LctsId { get; set; }
        //public string LctsTripId { get; set; }
        //public string LctsVin { get; set; }
        //public long? LctsTripStartTime { get; set; }
        //public long? LctsTripEndTime { get; set; }
        //public string LctsDriver1Id { get; set; }
        //public int LctsTripDistance { get; set; }
        //public int LctsDrivingTime { get; set; }
        //public int LctsFuelConsumption { get; set; }
        //public string LctsVehicleDrivingStatustype { get; set; }
        //public long LctsOdometerVal { get; set; }
        //public long LctsDistanceUntilNextService { get; set; }
        //public double LctsLatestReceivedPositionLattitude { get; set; }
        //public double LctsLatestReceivedPositionLongitude { get; set; }
        //public double LctsLatestReceivedPositionHeading { get; set; }
        ////   public int LctsLatestGeolocationAddressId { get; set; }
        //public double LctsStartPositionLattitude { get; set; }
        //public double LctsStartPositionLongitude { get; set; }
        //public double LctsStartPositionHeading { get; set; }
        //// public int LctsStartGeolocationAddressId { get; set; }
        //public long? LctsLatestProcessedMessageTimestamp { get; set; }
        //public string LctsVehicleHealthStatusType { get; set; }
        //public int LctsLatestWrningClass { get; set; }
        //public int LctsLatestWarningNumber { get; set; }
        //public string LctsLatestWarningType { get; set; }
        //public long? LctsLatestWarningTimestamp { get; set; }
        //public double LctsLatestWarningPositionLatitude { get; set; }
        //public double LctsLatestWarningPositionLongitude { get; set; }
        //// public int LctsLatestWarningGeolocationAddressId { get; set; }
        //public int LatgeoaddLatestGeolocationAddressId { get; set; }
        //public string LatgeoaddLatestGeolocationAddress { get; set; }
        //public int StageoaddStartGeolocationAddressId { get; set; }
        //public string StageoaddStartGeolocationAddress { get; set; }
        // public int WangeoaddLatestWarningGeolocationAddressId { get; set; }
        // public string WangeoaddLatestWarningGeolocationAddress { get; set; }

        //warningdata
        public int WarningId { get; set; }
        public string DriverName { get; set; }
        public string WarningAddress { get; set; }
        public int WarningAddressId { get; set; }
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

        public string WarningName { get; set; } = string.Empty;//from dtcwarning table
        public string WarningAdvice { get; set; } = string.Empty;
        public byte[] Icon { get; set; }
        public int IconId { get; set; }
        public string IconName { get; set; }
        public string ColorName { get; set; }
    }
}
