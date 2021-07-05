using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleSummary
    {
        public string VIN { get; set; }
        public string VehicleName { get; set; }
        public string VehicleRegNo { get; set; }
        public string VehicleDrivingStatusKey { get; set; }
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
        public string Vin { get; set; }
        public string Name { get; set; }
        public string ActivatedTime { get; set; }
        public string DeactivatedTime { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverId { get; set; }
        public string Advice { get; set; }
        public string VehicleName { get; set; }
        public double WarningLat { get; set; }
        public double WarningLng { get; set; }
        public long? WarningTimestamp { get; set; }
        public string VehicleDrivingStatusEnum { get; set; }
        public string VehicleHealthStatusEnum { get; set; }
        public string WarningTypeEnum { get; set; }
        public string Driver1Id { get; set; }
        public long? LastestProcessedMessageTimestamp { get; set; }
        public int WarningClass { get; set; }
        public int WarningNumber { get; set; }
    }
    public class VehicleHealthStatus
    {
        public List<VehicleHealthWarning> CurrentWarning { get; set; }
        public List<VehicleHealthWarning> HistoryWarning { get; set; }
        public VehicleSummary VehicleSummary { get; set; }
    }


}
