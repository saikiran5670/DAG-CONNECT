using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleSummary
    {
        public string VIN { get; set; }
        public string VehicleName { get; set; }
        public string VehicleRegNo { get; set; }
        public string DrivingStatus { get; set; }
        public string VehicleDrivingStatusEnum { get; set; }
        public int Alert { get; set; }
        public string Address { get; set; }
        public long? FromDate { get; set; }
        public long? ToDate { get; set; }
        public string WarningType { get; set; }
        public double LastLatitude { get; set; }
        public double LastLongitude { get; set; }
    }
    public class VehicleHealthWarning
    {
        public string Name { get; set; }
        public string ActivatedTime { get; set; }
        public string DeactivatedTime { get; set; }
        public string DriverName { get; set; }
        public string Advice { get; set; }
        public string VehicleName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class VehicleHealthStatus
    {
        public List<VehicleHealthWarning> CurrentWarning { get; set; }
        public List<VehicleHealthWarning> HistoryWarning { get; set; }
        public VehicleSummary VehicleSummary { get; set; }
    }


}
