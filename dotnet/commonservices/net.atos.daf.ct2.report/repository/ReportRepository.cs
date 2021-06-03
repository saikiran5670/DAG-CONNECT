using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.report.repository
{
    public class ReportRepository : IReportRepository
    {

        private readonly IDataAccess _dataAccess;
        private static readonly log4net.ILog _log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }
    }
}
