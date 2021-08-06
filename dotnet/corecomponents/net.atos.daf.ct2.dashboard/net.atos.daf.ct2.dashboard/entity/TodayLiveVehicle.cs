using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class TodayLiveVehicleResponse
    {
        public string TodayVin { get; set; }
        public double Distance { get; set; }
        public long DrivingTime { get; set; }
        public int DriverCount { get; set; }
        public int TodayActiveVinCount { get; set; }
        public long TodayTimeBasedUtilizationRate { get; set; }
        public long TodayDistanceBasedUtilization { get; set; }
        public int CriticleAlertCount { get; set; }
        public string YesterdayVin { get; set; }
        public int YesterdayActiveVinCount { get; set; }
        public long YesterDayTimeBasedUtilizationRate { get; set; }
        public long YesterDayDistanceBasedUtilization { get; set; }
    }

    public class TodayLiveVehicleRequest
    {
        public long TodayDateTime { get; set; }
        public long YesterdayDateTime { get; set; }
        public List<string> VINs { get; set; }
    }
}
