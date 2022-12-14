using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class EcoScoreKPIInfoDataServiceResponse
    {
        public List<KPIInfo> KPIInfo { get; set; }
    }

    public class EcoScoreChartInfoDataServiceResponse
    {
        public List<ChartInfo> ChartInfo { get; set; }
    }

    public class KPIInfo
    {
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
        public KPI AnticipationScore { get; set; }
        public KPI AverageDistancePerDay { get; set; }
        public KPI AverageGrossWeight { get; set; }
        public KPI AverageSpeed { get; set; }
        public KPI AverageDrivingSpeed { get; set; }
        public KPI BrakingDuration { get; set; }
        public KPI BrakingPercentage { get; set; }
        public KPI BrakingScore { get; set; }
        public KPI CruiseControlUsage { get; set; }
        public KPI CruiseControlUsage3050kmph { get; set; }
        public KPI CruiseControlUsage5075kmph { get; set; }
        public KPI CruiseControlUsage75kmph { get; set; }
        public KPI Distance { get; set; }
        public double Ecoscore { get; set; }
        public KPI FuelConsumption { get; set; }
        public KPI HarshBrakeDuration { get; set; }
        public KPI HarshBrakePercentage { get; set; }
        public KPI HeavyThrottlingDuration { get; set; }
        public KPI HeavyThrottlingPercentage { get; set; }
        public KPI IdleDuration { get; set; }
        public KPI IdlingPercentage { get; set; }
        public int? NumberOfTrips { get; set; }
        public int? NumberOfVehicles { get; set; }
        public KPI PTODuration { get; set; }
        public KPI PTOPercentage { get; set; }
    }

    public class ChartInfo
    {
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
        public KPI AnticipationScore { get; set; }
        public KPI BrakingScore { get; set; }
        public double Ecoscore { get; set; }
        public KPI FuelConsumption { get; set; }
    }

    public class KPI
    {
        public double? Total { get; set; }
        public double? Count { get; set; }

        public KPI(double? total, double? count)
        {
            Total = total;
            Count = count;
        }
    }
}
