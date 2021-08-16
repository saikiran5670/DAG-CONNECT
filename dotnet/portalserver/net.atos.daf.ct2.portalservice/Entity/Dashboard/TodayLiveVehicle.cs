using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Entity.Dashboard
{
    public class TodayLiveVehicleResponse
    {
        public double Distance { get; set; }
        public long DrivingTime { get; set; }
        public int VehicleCount { get; set; }
        public int DriverCount { get; set; }
        public int CriticleAlertCount { get; set; }
        public int ActiveVehicles { get; set; }
        public int TimeBaseUtilization { get; set; }
        public int DistanceBaseUtilization { get; set; }
    }

    public class TodayLiveVehicleRequest
    {
        public List<string> VINs { get; set; }
    }
}
