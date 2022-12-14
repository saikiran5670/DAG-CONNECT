using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.entity.fleetFuel;

namespace net.atos.daf.ct2.reports
{
    public interface IReportManager
    {
        Task<bool> CheckIfUserPreferencesExist(int reportId, int accountId, int organizationId);
        Task<IEnumerable<UserPreferenceReportDataColumn>> GetReportUserPreference(int reportId, int accountId, int organizationId);
        Task<IEnumerable<UserPreferenceReportDataColumn>> GetRoleBasedDataColumn(int reportId, int accountId, int roleId, int organizationId, int contextOrgId);
        Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest);
        Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList);
        Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripFilter,
                                                                    bool isLiveFleetRequired = true);
        Task<List<DriversActivities>> GetDriverActivity(DriverActivityFilter driverActivityFilter);
        Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter driverActivityFilter);
        Task<List<DriverActivityChart>> GetDriversActivityChartDetails(DriverActivityChartFilter driverActivityFilter);
        Task<IEnumerable<ReportDetails>> GetReportDetails();
        Task<List<Driver>> GetDriversByVIN(long startDateTime, long endDateTime, List<string> vin, int organizationId);
        Task<List<Driver>> GetDriversByVINForEcoScore(long startDateTime, long endDateTime, List<string> vin, int organizationId);
        Task<int> CreateEcoScoreProfile(EcoScoreProfileDto dto);
        Task<int> GetEcoScoreProfilesCount(int orgId);
        Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId);
        Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId);
        Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto, bool isAdminRights);
        Task<int> DeleteEcoScoreProfile(int profileId, bool isAdminRights);
        Task<object> GetReportSearchParameterByVIN(int reportID, long startDateTime, long endDateTime, List<string> vin);
        Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter fleetFilter);
        Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter tripFilters);
        Task<List<EcoScoreReportByAllDrivers>> GetEcoScoreReportByAllDrivers(EcoScoreReportByAllDriversRequest request);
        Task<bool> CreateReportUserPreference(ReportUserPreferenceCreateRequest request);
        Task<bool> CheckIfReportUserPreferencesExist(int reportId, int accountId, int organizationId, int[] featureIds);
        Task<IEnumerable<ReportUserPreference>> GetReportUserPreferences(int reportId, int accountId, int organizationId, int[] featureIds);
        Task<IEnumerable<ReportUserPreference>> GetPrivilegeBasedReportUserPreferences(int reportId, int accountId, int roleId, int organizationId, int contextOrgId, int[] featureId);
        Task<IEnumerable<ReportUserPreference>> GetReportDataAttributes(int[] featureIds, int reportId);
        Task<IEnumerable<int>> GetReportFeatureId(int reportId);
        Task<IEnumerable<ReportUserPreference>> GetReportDataAttributes(List<int> reportIds);
        Task<SubReportDto> CheckIfSubReportExist(int reportId);
        Task<List<EcoScoreReportCompareDrivers>> GetEcoScoreReportCompareDrivers(EcoScoreReportCompareDriversRequest request);
        Task<List<EcoScoreCompareReportAtttributes>> GetEcoScoreCompareReportAttributes(int reportId, int targetProfileId);
        Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportSingleDriver(EcoScoreReportSingleDriverRequest request);
        Task<List<EcoScoreSingleDriverBarPieChart>> GetEcoScoreAverageGrossWeightChartData(EcoScoreReportSingleDriverRequest request);
        Task<List<EcoScoreSingleDriverBarPieChart>> GetEcoScoreAverageDrivingSpeedChartData(EcoScoreReportSingleDriverRequest request);
        Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportTrendlineData(EcoScoreReportSingleDriverRequest request);
        Task<List<AlertCategory>> GetAlertCategoryList();
        Task<List<FleetFuelDetails>> GetFleetFuelDetailsByVehicle(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuelDetailsByDriver>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters);
        Task<List<FilterProperty>> GetAlertLevelList();
        Task<List<FilterProperty>> GetHealthStatusList();
        Task<List<FilterProperty>> GetOtherFilter();
        Task<List<FleetOverviewDetails>> GetFleetOverviewDetails(FleetOverviewFilter fleetOverviewFilter);
        Task<IEnumerable<string>> GetEnumList(IEnumerable<int> alertFeatureIds);
        Task<List<FleetOverviewDetails>> GetFleetOverviewDetails_NeverMoved(FleetOverviewFilter fleetOverviewFilter);
        Task<List<FleetOverviewDetails>> GetFleetOverviewDetails_NeverMoved_NoWarnings(FleetOverviewFilter fleetOverviewFilter);
        Task<EcoScoreKPIInfoDataServiceResponse> GetKPIInfo(EcoScoreDataServiceRequest request);
        Task<EcoScoreChartInfoDataServiceResponse> GetChartInfo(EcoScoreDataServiceRequest request);
        Task<List<DriverFilter>> GetDriverList(List<string> vins, int organizationId);
        Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForVehicleGraphs(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForDriverGraphs(FleetFuelFilter fleetFuelFilters);

        Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters, bool isLiveFleetRequired = true);
        Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByDriver(FleetFuelFilterDriver fleetFuelFilters);

        Task<List<VehicleHealthResult>> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest);
        Task<List<WarningDetails>> GetWarningDetails(List<int> warningClass, List<int> warningNumber, string lngCode);
        Task<List<DriverDetails>> GetDriverDetails(List<string> driverIds, int organizationId);
        Task<List<AlertType>> GetAlertTypeList();

        #region Fuel Deviation Report
        Task<IEnumerable<FuelDeviation>> GetFilteredFuelDeviation(FuelDeviationFilter fuelDeviationFilters);
        Task<IEnumerable<FuelDeviationCharts>> GetFuelDeviationCharts(FuelDeviationFilter fuelDeviationFilters);
        #endregion
        Task<IEnumerable<LogbookTripAlertDetails>> GetLogbookSearchParameter(List<string> vins, List<int> featureIds);
        Task<List<LogbookDetails>> GetLogbookDetails(LogbookDetailsFilter logbookFilter);
        Task<List<AlertThresholdDetails>> GetThresholdDetails(List<int> alertId, List<string> alertLevel);
        Task<List<FilterProperty>> GetAlertLevelList(List<string> enums);
        Task<List<AlertCategory>> GetAlertCategoryList(List<string> enums);
        Task<IEnumerable<EnumTranslation>> GetAlertCategory();

        #region Fuel Benchmark Report
        // Task<IEnumerable<FuelBenchmark>> GetFuelBenchmarks(FuelBenchmark fuelBenchmarkFilter);
        Task<FuelBenchmarkDetails> GetFuelBenchmarkDetails(FuelBenchmarkFilter fuelBenchmarkFilter);
        #endregion
        #region Vehicle Performance Report
        Task<VehiclePerformanceChartTemplate> GetVehPerformanceChartTemplate(VehiclePerformanceRequest vehiclePerformanceRequest);
        Task<VehiclePerformanceSummary> GetVehPerformanceSummaryDetails(string vin);
        Task<VehiclePerformanceData> GetVehPerformanceBubbleChartData(VehiclePerformanceRequest vehiclePerformanceRequest);
        Task<List<VehPerformanceProperty>> GetVehPerformanceType();
        #endregion
    }
}
