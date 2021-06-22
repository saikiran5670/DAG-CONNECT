using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.repository
{
    public interface IReportRepository
    {
        Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId, int accountIdint, int organizationId);
        Task<IEnumerable<UserPrefernceReportDataColumn>> GetRoleBasedDataColumn(int reportId, int accountIdint, int organizationId);
        Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest);
        Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList);
        Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripEntityRequest,
                                                                    bool IsLiveFleetRequired = true);
        Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter activityFilters);
        Task<IEnumerable<ReportDetails>> GetReportDetails();
        Task<List<Driver>> GetDriversByVIN(long StartDateTime, long EndDateTime, List<string> VIN);
        Task<bool> CreateEcoScoreProfile(EcoScoreProfileDto dto);
        Task<int> GetEcoScoreProfilesCount(int orgId);
        Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId);
        Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId);
        Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto);
        Task<bool> CheckEcoScoreProfileIsexist(int? organizationId, string name);
        Task<int> DeleteEcoScoreProfile(int profileId);
        Task<string> IsEcoScoreProfileBasicOrAdvance(int profileId);
        Task<string> GetProfileName(int profileId);
        Task<bool> GetGlobalProfile(int profileId);
        Task<object> GetReportSearchParameterByVIN(int ReportID, long StartDateTime, long EndDateTime, List<string> VIN, [Optional] string ReportView);
        Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter FleetUtilizationFilters);
        Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter tripFilters);
        Task<List<EcoScoreReportByAllDrivers>> GetEcoScoreReportByAllDrivers(EcoScoreReportByAllDriversRequest request);
        Task<EcoScoreKPIRanking> GetEcoScoreTargetProfileKPIValues(EcoScoreReportByAllDriversRequest request);
        Task<bool> UpdateEcoScoreTargetProfile(EcoScoreReportByAllDriversRequest request);
    }
}
