using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports
{
    public interface IReportManager
    {
        Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId, int accountId, int organizationId);
        Task<IEnumerable<UserPrefernceReportDataColumn>> GetRoleBasedDataColumn(int reportId, int accountIdint, int organizationId);
        Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest);
        Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList);
        Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripFilter);
        Task<List<DriversActivities>> GetDriverActivity(DriverActivityFilter driverActivityFilter);
        Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter driverActivityFilter);
        Task<IEnumerable<ReportDetails>> GetReportDetails();
        Task<List<Driver>> GetDriversByVIN(long StartDateTime, long EndDateTime, List<string> VIN);
        Task<bool> CreateEcoScoreProfile(EcoScoreProfileDto dto);
        Task<int> GetEcoScoreProfilesCount(int orgId);
        Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId);
        Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId);
        Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto);
        Task<int> DeleteEcoScoreProfile(int profileId, bool isAdminRights);
        Task<string> GetProfileName(int profileId);
        Task<object> GetReportSearchParameterByVIN(int ReportID, long StartDateTime, long EndDateTime, List<string> VIN);
        Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter FleetFilter);
    }
}
