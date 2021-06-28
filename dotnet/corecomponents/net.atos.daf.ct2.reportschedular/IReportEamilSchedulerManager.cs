using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public interface IReportEmailSchedulerManager
    {
        Task<List<ReportEmailDetail>> SendReportEmail();
    }
}
