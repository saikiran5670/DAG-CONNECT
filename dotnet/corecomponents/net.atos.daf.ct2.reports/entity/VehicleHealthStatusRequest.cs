namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleHealthStatusRequest
    {
        public string VIN { get; set; }
        public string TripId { get; set; } = string.Empty;
        public int Days { get; set; }
        //public long? FromDate { get; set; }
        // public long? ToDate { get; set; }
        public string WarningType { get; set; }
        public string LngCode { get; set; } = string.Empty;
    }

    public class WarningDetails
    {

        public int WarningClass { get; set; }
        public int WarningNumber { get; set; }
        public string WarningName { get; set; }
        public string WarningAdvice { get; set; }
        public byte[] Icon { get; set; }
        public int IconId { get; set; }
        public string IconName { get; set; }
        public string ColorName { get; set; }

    }
    public class DriverDetails
    {

        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverStatus { get; set; }



    }

}
