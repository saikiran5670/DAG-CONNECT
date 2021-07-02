using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;

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
        Task<IEnumerable<ReportDetails>> GetReportDetails();
        Task<List<Driver>> GetDriversByVIN(long startDateTime, long endDateTime, List<string> vin);
        Task<bool> CreateEcoScoreProfile(EcoScoreProfileDto dto);
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
        Task<bool> CheckIfReportUserPreferencesExist(int reportId, int accountId, int organizationId);
        Task<IEnumerable<ReportUserPreference>> GetReportUserPreferences(int reportId, int accountId, int organizationId);
        Task<IEnumerable<ReportUserPreference>> GetPrivilegeBasedReportUserPreferences(int reportId, int accountId, int roleId, int organizationId, int contextOrgId);
        Task<List<EcoScoreReportCompareDrivers>> GetEcoScoreReportCompareDrivers(EcoScoreReportCompareDriversRequest request);
        Task<List<EcoScoreCompareReportAtttributes>> GetEcoScoreCompareReportAttributes(int reportId, int targetProfileId);
        Task<List<AlertCategory>> GetAlertCategoryList();
        Task<List<FleetFuelDetails>> GetFleetFuelDetailsByVehicle(FleetFuelFilter fleetFuelFilters);
        Task<List<FleetFuelDetails>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters);
        Task<List<FilterProperty>> GetAlertLevelList();
        Task<List<FilterProperty>> GetHealthStatusList();
        Task<List<FilterProperty>> GetOtherFilter();
    }
}
