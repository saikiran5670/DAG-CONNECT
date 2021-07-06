namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleHealthStatusRequest
    {
        public string VIN { get; set; }
        public long? FromDate { get; set; }
        public long? ToDate { get; set; }
        public string WarningType { get; set; }
        public string LngCode { get; set; }
    }

}
