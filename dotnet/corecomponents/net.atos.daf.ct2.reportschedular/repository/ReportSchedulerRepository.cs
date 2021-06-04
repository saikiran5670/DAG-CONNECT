using System;
using System.Collections.Generic;
using System.Text;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public class ReportSchedulerRepository : IReportSchedulerRepository
    {
        private readonly IDataAccess _dataAccess;
        private static readonly log4net.ILog _log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportSchedulerRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        
    }
}
