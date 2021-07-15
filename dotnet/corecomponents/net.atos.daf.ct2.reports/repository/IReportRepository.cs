﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.entity.fleetFuel;

namespace net.atos.daf.ct2.reports.repository
{
    public interface IReportRepository
    {
        Task<bool> CheckIfUserPreferencesExist(int reportId, int accountIdint, int organizationId);
        Task<IEnumerable<UserPreferenceReportDataColumn>> GetReportUserPreference(int reportId, int accountId, int organizationId);
        Task<IEnumerable<UserPreferenceReportDataColumn>> GetRoleBasedDataColumn(int reportId, int accountId, int roleId, int organizationId, int contextOrgId);
        Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest);
        Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList);
        Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripEntityRequest,
                                                                    bool isLiveFleetRequired = true);
        Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter activityFilters);
        Task<IEnumerable<ReportDetails>> GetReportDetails();
        Task<List<Driver>> GetDriversByVIN(long startDateTime, long endDateTime, List<string> vin);
        Task<List<DriverActivityChart>> GetDriversActivityChartDetails(DriverActivityChartFilter activityFilters);
        Task<int> CreateEcoScoreProfile(EcoScoreProfileDto dto);
        Task<int> GetEcoScoreProfilesCount(int orgId);
        Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId);
        Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId);
        Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto);
        Task<bool> CheckEcoScoreProfileIsExist(int organizationId, string name, int profileId);
        Task<int> DeleteEcoScoreProfile(int profileId);
        Task<string> IsEcoScoreProfileBasicOrAdvance(int profileId);
        Task<bool> GetGlobalProfile(int profileId);
        Task<object> GetReportSearchParameterByVIN(int ReportID, long StartDateTime, long EndDateTime, List<string> VIN, [Optional] string ReportView);
        Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter FleetUtilizationFilters);
        Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter tripFilters);
        Task<List<EcoScoreReportByAllDrivers>> GetEcoScoreReportByAllDrivers(EcoScoreReportByAllDriversRequest request);
        Task<EcoScoreKPIRanking> GetEcoScoreTargetProfileKPIValues(int targetProfileId);
        Task<bool> UpdateEcoScoreTargetProfile(EcoScoreReportByAllDriversRequest request);
        Task<bool> CreateReportUserPreference(ReportUserPreferenceCreateRequest request);
        Task<bool> CheckIfReportUserPreferencesExist(int reportId, int accountId, int organizationId);
        Task<IEnumerable<ReportUserPreference>> GetReportUserPreferences(int reportId, int accountId, int organizationId);
        Task<IEnumerable<ReportUserPreference>> GetPrivilegeBasedReportUserPreferences(int reportId, int accountId, int roleId, int organizationId, int contextOrgId);
        Task<List<EcoScoreReportCompareDrivers>> GetEcoScoreReportCompareDrivers(EcoScoreReportCompareDriversRequest request);
        Task<List<EcoScoreCompareReportAtttributes>> GetEcoScoreCompareReportAttributes(int reportId, int targetProfileId);
        Task<List<AlertCategory>> GetAlertCategoryList();
        #region FleetFuel
        Task<List<CO2Coefficient>> GetCO2CoEfficientData();
        Task<List<IdlingConsumption>> GetIdlingConsumptionData(string languageCode);
        Task<List<AverageTrafficClassification>> GetAverageTrafficClassificationData(string languageCode);
        Task<List<FleetFuelDetails>> GetFleetFuelDetailsByVehicle(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuelDetailsByDriver>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForVehicleGraphs(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForDriverGraphs(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByDriver(FleetFuelFilterDriver fleetFuelFilters);
        #endregion
        Task<List<FilterProperty>> GetAlertLevelList();
        Task<List<FilterProperty>> GetHealthStatusList();
        Task<List<FilterProperty>> GetOtherFilter();
        Task<List<FleetOverviewDetails>> GetFleetOverviewDetails(FleetOverviewFilter fleetOverviewFilter);
        Task<dynamic> GetKPIInfo(EcoScoreDataServiceRequest request);
        Task<dynamic> GetChartInfo(EcoScoreDataServiceRequest request);
        Task<List<DriverFilter>> GetDriverList(List<string> vins);

        Task<List<VehicleHealthResult>> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest);
        Task<List<WarningDetails>> GetWarningDetails(List<int> warningClass, List<int> warningNumber, string lngCode);
        Task<List<DriverDetails>> GetDriverDetails(List<string> driverIds, int organizationId);
        #region Fuel Deviation Report
        Task<IEnumerable<FuelDeviation>> GetFilteredFuelDeviation(FuelDeviationFilter fuelDeviationFilters);
        #endregion
        #region LogBook
        Task<IEnumerable<LogbookSearchFilter>> GetLogbookSearchParameter(List<string> vins);
        Task<List<FilterProperty>> GetAlertLevelList(List<string> enums);
        Task<List<AlertCategory>> GetAlertCategoryList(List<string> enums);
        Task<List<LogbookDetailsFilter>> GetLogbookDetails(LogbookFilter logbookFilter);
        #endregion

        #region Fuel Benchmark Report
        //  Task<IEnumerable<FuelBenchmark>> GetFuelBenchmarks(FuelBenchmark fuelBenchmarkFilter);
        Task<IEnumerable<Ranking>> GetFuelBenchmarkRanking(FuelBenchmarkFilter fuelBenchmarkFilter);
        Task<FuelBenchmarkConsumption> GetFuelBenchmarkDetail(FuelBenchmarkFilter fuelBenchmarkFilter);
        #endregion
    }
}
