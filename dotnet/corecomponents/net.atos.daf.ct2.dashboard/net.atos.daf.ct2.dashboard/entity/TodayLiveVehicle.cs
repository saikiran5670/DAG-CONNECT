using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class TodayLiveVehicleResponse
    {
        public string TodayVin { get; set; }
        public long Distance { get; set; }
        public long DrivingTime { get; set; }
        public int DriverCount { get; set; }
        public int TodayActiveVinCount { get; set; }
        public long TodayTimeBasedUtilizationRate { get; set; }
        public long TodayDistanceBasedUtilization { get; set; }
        public int CriticleAlertCount { get; set; }
        public string YesterdayVin { get; set; }
        public int YesterdayActiveVinCount { get; set; }
        public long YesterdayTimeBasedUtilizationRate { get; set; }
        public long YesterdayDistanceBasedUtilization { get; set; }
    }
    public class TodayLiveVehicleData
    {
        public string TodayVin { get; set; }
        public long TodayDistance { get; set; }
        public long TodayDrivingTime { get; set; }
        public int TodayAlertCount { get; set; }
        public string YesterdayVin { get; set; }
        public long YesterdayDistance { get; set; }
        public long YesterdayDrivingTime { get; set; }
    }

    public class TodayLiveVehicleRequest
    {
        public long TodayDateTime { get; set; }
        public long YesterdayDateTime { get; set; }
        public long TomorrowDateTime { get; set; }
        public long DayDeforeYesterdayDateTime { get; set; }
        public List<string> VINs { get; set; }
    }
}
