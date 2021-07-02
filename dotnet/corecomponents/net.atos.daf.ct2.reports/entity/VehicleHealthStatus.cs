using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehicleSummary
    {
        public string VIN { get; set; }
        public string VehicleName { get; set; }
        public string VehicleRegNo { get; set; }
        public string DrivingStatus { get; set; }
        public int Alert { get; set; }
        public string Address { get; set; }
        public long? FromDate { get; set; }
        public long? ToDate { get; set; }
        public string WarningType { get; set; }
        public long LastLatitude { get; set; }
        public long LastLongitude { get; set; }
    }

    public class CurrentHealthStatus
    {
        public List<Warning> CurrentWarning { get; set; }
    }

    public class Warning
    {
        public string Name { get; set; }
        public string ActivatedTime { get; set; }
        public string DeactivatedTime { get; set; }
        public string DriverName { get; set; }
        public string Advice { get; set; }
        public string VehicleName { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
    }

    public class VehicleHealthStatusHitory
    {
        public List<Warning> WarningHistory { get; set; }
    }

    public class VehicleHealthStatus
    {
        // public CurrentHealthStatus Current { get; set; }
        public VehicleHealthStatusHitory Hitory { get; set; }
        public VehicleSummary VehicleSummary { get; set; }
    }


}
