namespace net.atos.daf.ct2.reports.entity.fleetOverview
{
    public class VehicleHealthStatusHistoryRequest : VehicleHealthStatusRequest
    {
        public long FromDate { get; set; }
        public long ToDate { get; set; }
        public string WarningType { get; set; }
    }
}
