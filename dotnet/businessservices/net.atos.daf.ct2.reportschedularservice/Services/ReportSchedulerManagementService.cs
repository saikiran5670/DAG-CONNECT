using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using net.atos.daf.ct2.reportscheduler;

namespace net.atos.daf.ct2.reportschedulerservice.Services
{
    public class ReportSchedulerManagementService
    {
        private ILog _logger;
        private readonly IReportSchedulerManager _reportSchedulerManager;
        private readonly Mapper _mapper;
        public ReportSchedulerManagementService(IReportSchedulerManager reportSchedulerManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _reportSchedulerManager = reportSchedulerManager;
            _mapper = new Mapper();
        }
    }
}
