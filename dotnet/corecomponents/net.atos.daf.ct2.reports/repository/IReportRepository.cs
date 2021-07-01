using System.Collections.Generic;
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
        Task<List<Driver>> GetDriversByVIN(long StartDateTime, long EndDateTime, List<string> VIN);
        Task<bool> CreateEcoScoreProfile(EcoScoreProfileDto dto);
        Task<int> GetEcoScoreProfilesCount(int orgId);
        Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId);
        Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId);
        Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto);
        Task<bool> CheckEcoScoreProfileIsExist(int? organizationId, string name);
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
        Task<List<AlertCategory>> GetAlertCategoryList();
        Task<List<AlertLevel>> GetAlertLevelList();

        #region FleetFuel
        Task<List<CO2CoEfficient>> GetCO2CoEfficientData();
        Task<List<IdlingConsumption>> GetIdlingConsumptionData();
        Task<List<AverageTrafficClassification>> GetAverageTrafficClassificationData();
        Task<List<FleetFuelDetails>> GetFleetFuelDetailsByVehicle(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuelDetails>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters);
        #endregion
        Task<List<FilterProperty>> GetAlertLevelList();
        Task<List<FilterProperty>> GetHealthStatusList();
        Task<List<FilterProperty>> GetOtherFilter();
    }
}
