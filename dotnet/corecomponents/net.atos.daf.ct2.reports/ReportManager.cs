using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.repository;
using System.Linq;

namespace net.atos.daf.ct2.reports
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _reportRepository;

        public ReportManager(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        #region Select User Preferences
        public Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId,
                                                                                                  int accountId,
                                                                                                  int organizationId)
        {
            return _reportRepository.GetUserPreferenceReportDataColumn(reportId, accountId, organizationId);
        }

        public Task<IEnumerable<UserPrefernceReportDataColumn>> GetRoleBasedDataColumn(int reportId,
                                                                                                  int accountId,
                                                                                                  int organizationId)
        {
            return _reportRepository.GetRoleBasedDataColumn(reportId, accountId, organizationId);
        }
        #endregion

        #region Create User Preferences
        public async Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest)
        {
            return await _reportRepository.CreateUserPreference(objUserPreferenceRequest);
        }
        #endregion

        #region Get Vins from data mart trip_statistics
        public Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList)
        {
            return _reportRepository.GetVinsFromTripStatistics(vinList);
        }
        #endregion

        #region Trip Report Table Details

        public async Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripFilter)
        {
            return await _reportRepository.GetFilteredTripDetails(tripFilter);
        }

        #endregion

        #region Driver Time management Report
        /// <summary>
        /// Fetch Multiple Drivers activity data and group by name with all type duraion aggregate
        /// </summary>
        /// <param name="DriverActivityFilter">Filters for driver activity with VIN and Driver ID </param>
        /// <returns></returns>
        public async Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter DriverActivityFilter)
        {
            List<DriversActivities> driverActivities = await _reportRepository.GetDriversActivity(DriverActivityFilter);
            List<DriversActivities> combineDriverActivities = new List<DriversActivities>();
            combineDriverActivities = driverActivities.GroupBy(activityGroup => activityGroup.DriverId)
                                                      .Select(activityItem => new DriversActivities
                                                      {
                                                          DriverName = activityItem.FirstOrDefault().DriverName,
                                                          DriverId = activityItem.FirstOrDefault().DriverId,
                                                          ActivityDate = activityItem.FirstOrDefault().ActivityDate,
                                                          Code = activityItem.FirstOrDefault().Code,
                                                          VIN = activityItem.FirstOrDefault().VIN,
                                                          AvailableTime = activityItem.Sum(c => c.AvailableTime),
                                                          DriveTime = activityItem.Sum(c => c.DriveTime),
                                                          RestTime = activityItem.Sum(c => c.RestTime),
                                                          WorkTime = activityItem.Sum(c => c.WorkTime),
                                                          ServiceTime = activityItem.Sum(c => c.ServiceTime),
                                                      }).ToList();
            return combineDriverActivities;

        }

        /// <summary>
        /// Fetch Single driver activities data by Day group
        /// </summary>
        /// <param name="DriverActivityFilter">Filters for driver activity with VIN and Driver ID </param>
        /// <returns></returns>
        public async Task<List<DriversActivities>> GetDriverActivity(DriverActivityFilter DriverActivityFilter)
        {
            return await _reportRepository.GetDriversActivity(DriverActivityFilter);
        }

        #endregion
    }
}
