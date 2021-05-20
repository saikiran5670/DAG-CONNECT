using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId, int accountId)
        {
            return _reportRepository.GetUserPreferenceReportDataColumn(reportId, accountId);
        }
        #endregion

        #region Create User Preferences
        public async Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest)
        {
            return await _reportRepository.CreateUserPreference(objUserPreferenceRequest);
        }
        #endregion

        #region Get Vins from data mart trip_statistics
        //This code is not in use, may require in future use.
        public Task<IEnumerable<string>> GetVinsFromTripStatistics(long fromDate, long toDate,
                                                                   IEnumerable<string> vinList)
        {
            return _reportRepository.GetVinsFromTripStatistics(fromDate, toDate, vinList);
        }
        #endregion
    }
}
