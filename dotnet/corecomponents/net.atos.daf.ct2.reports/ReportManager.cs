using net.atos.daf.ct2.reports.repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _reportRepository;

        public GeofenceManager(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        #region Select User Preferences

        #endregion
    }
}
