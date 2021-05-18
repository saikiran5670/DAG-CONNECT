using log4net;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reportservice.entity;
using System.Reflection;

namespace net.atos.daf.ct2.reportservice.Services
{
    public class ReportManagementService
    {
        private ILog _logger;
        private readonly IReportManager _reportManager;
        private readonly Mapper _mapper;
        public ReportManagementService(IReportManager reportManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _reportManager = reportManager;
            _mapper = new Mapper();
        }
    }
}
