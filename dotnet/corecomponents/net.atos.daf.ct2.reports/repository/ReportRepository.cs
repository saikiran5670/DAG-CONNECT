using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDataAccess _dataAccess;        
        private static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;            
        }

        #region Select User Preferences

        #endregion
    }
}
