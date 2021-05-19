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
        //public async  Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId, int accountId)
        //{
        //   return await _reportRepository.GetUserPreferenceReportDataColumn(reportId, accountId);
        //}
        #endregion
    }
}
