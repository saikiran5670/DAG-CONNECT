using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public enum ProfileType
    {
        Default = 'D',
        Advanced = 'A',
        None = 'N'
    }

    public class EcoScoreProfileDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ActionedBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
        public ProfileType Type { get; set; }
    }

    public class EcoScoreProfileKPI
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string SectionDescription { get; set; }
        public int KPIId { get; set; }
        public string KPIName { get; set; }
        public string LimitType { get; set; }
        public double LimitValue { get; set; }
        public double TargetValue { get; set; }
        public double LowerValue { get; set; }
        public double UpperValue { get; set; }
        public string RangeValueType { get; set; }
        public double MaxUpperValue { get; set; }
        public int SequenceNo { get; set; }
    }

    public class EcoScoreReportByAllDrivers
    {
        public int Ranking { get; set; }
        public string DriverName { get; set; }
        public string DriverId { get; set; }
        public double EcoScoreRanking { get; set; }
        public string EcoScoreRankingColor { get; set; }
    }

    public class EcoScoreReportByAllDriversRequest
    {
        public int ReportId { get; set; }
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
        public double MinTripDistance { get; set; }
        public double MinDriverTotalDistance { get; set; }
        public int OrgId { get; set; }
        public int AccountId { get; set; }
        public int TargetProfileId { get; set; }
    }

    public class EcoScoreKPIRanking
    {
        public int ProfileId { get; set; }
        public string KPIName { get; set; }
        public double MinValue { get; set; }
        public double TargetValue { get; set; }
    }

    public class EcoScoreReportCompareDriversRequest
    {
        public int ReportId { get; set; }
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
        public List<string> DriverIds { get; set; }
        public double MinTripDistance { get; set; }
        public double MinDriverTotalDistance { get; set; }
        public int OrgId { get; set; }
        public int AccountId { get; set; }
        public int TargetProfileId { get; set; }
    }

    public class EcoScoreCompareReportAtttributes
    {
        public int DataAttributeId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int[] SubDataAttributes { get; set; }
        public string DBColumnName { get; set; }
        public string LimitType { get; set; }
        public double LimitValue { get; set; }
        public double TargetValue { get; set; }
        public string RangeValueType { get; set; }
    }

    public class EcoScoreReportCompareDrivers
    {
        public string DriverName { get; set; }
        public string DriverId { get; set; }
        public double AverageGrossweight { get; set; }
        public double Distance { get; set; }
        public double NumberOfTrips { get; set; }
        public double NumberOfVehicles { get; set; }
        public double AverageDistancePerDay { get; set; }
        public double EcoScore { get; set; }
        public double FuelConsumption { get; set; }
        public double CruiseControlUsage { get; set; }
        public double CruiseControlUsage30 { get; set; }
        public double CruiseControlUsage50 { get; set; }
        public double CruiseControlUsage75 { get; set; }
        public double PTOUsage { get; set; }
        public double PTODuration { get; set; }
        public double AverageDrivingSpeed { get; set; }
        public double AverageSpeed { get; set; }
        public double HeavyThrottling { get; set; }
        public double HeavyThrottleDuration { get; set; }
        public double Idling { get; set; }
        public double IdleDuration { get; set; }
        public double BrakingScore { get; set; }
        public double HarshBraking { get; set; }
        public double HarshBrakeDuration { get; set; }
        public double BrakeDuration { get; set; }
        public double Braking { get; set; }
        public double AnticipationScore { get; set; }
    }

    public class EcoScoreReportSingleDriverRequest
    {
        public int ReportId { get; set; }
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
        public string DriverId { get; set; }
        public double MinTripDistance { get; set; }
        public double MinDriverTotalDistance { get; set; }
        public int OrgId { get; set; }
        public int AccountId { get; set; }
        public int TargetProfileId { get; set; }
        public string UoM { get; set; }
    }
    public class EcoScoreReportSingleDriver
    {
        public string HeaderType { get; set; }
        public string VIN { get; set; }
        public string VehicleName { get; set; }
        public string RegistrationNo { get; set; }
        public string DriverId { get; set; }
        public double AverageGrossweight { get; set; }
        public double Distance { get; set; }
        public double NumberOfTrips { get; set; }
        public double NumberOfVehicles { get; set; }
        public double AverageDistancePerDay { get; set; }
        public double EcoScore { get; set; }
        public double FuelConsumption { get; set; }
        public double CruiseControlUsage { get; set; }
        public double CruiseControlUsage30 { get; set; }
        public double CruiseControlUsage50 { get; set; }
        public double CruiseControlUsage75 { get; set; }
        public double PTOUsage { get; set; }
        public double PTODuration { get; set; }
        public double AverageDrivingSpeed { get; set; }
        public double AverageSpeed { get; set; }
        public double HeavyThrottling { get; set; }
        public double HeavyThrottleDuration { get; set; }
        public double Idling { get; set; }
        public double IdleDuration { get; set; }
        public double BrakingScore { get; set; }
        public double HarshBraking { get; set; }
        public double HarshBrakeDuration { get; set; }
        public double BrakeDuration { get; set; }
        public double Braking { get; set; }
        public double AnticipationScore { get; set; }
    }

    public class EcoScoreSingleDriverBarPieChart
    {
        public string VIN { get; set; }
        public string VehicleName { get; set; }
        public string X_Axis { get; set; }
        public double Y_Axis { get; set; }
        public double Distance { get; set; }
    }

    public enum RankingColor
    {
        Red,
        Green,
        Amber
    }
    public enum LimitType
    {
        Min = 'N',
        Max = 'X',
        None = 'O'
    }
    public enum OverallPerformance
    {
        EcoScore,
        FuelConsumption,
        AnticipationScore,
        BrakingScore
    }
    public enum UoM
    {
        Metric,
        Imperial
    }
}
