using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity.fleetOverview
{
    public class Summary
    {
        public string VIN { get; set; }
        public string VehicleName { get; set; }
        public string VehicleRegNo { get; set; }
        public string DrivingStatus { get; set; }
        public int Alert { get; set; }
        public string Address { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string WarningType { get; set; }
    }

    public class CurrentWarning
    {
        public string Name { get; set; }
        public string ActivatedTime { get; set; }
        public string DriverName { get; set; }
        public string Advice { get; set; }
        public string DeactivatedTime { get; set; }
    }

    public class Current
    {
        public Summary Summary { get; set; }
        public List<CurrentWarning> CurrentWarning { get; set; }
    }

    public class WarningHistory
    {
        public string Name { get; set; }
        public string ActivatedTime { get; set; }
        public string DeactivatedTime { get; set; }
        public string DriverName { get; set; }
        public string Advice { get; set; }
    }

    public class Hitory
    {
        public Summary Summary { get; set; }
        public List<WarningHistory> WarningHistory { get; set; }
    }

    public class VehicleHealthStatus
    {
        public Current Current { get; set; }
        public Hitory Hitory { get; set; }
    }


}
