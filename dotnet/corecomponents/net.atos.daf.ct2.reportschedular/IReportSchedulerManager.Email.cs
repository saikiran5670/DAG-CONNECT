using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial interface IReportSchedulerManager
    {
        Task<bool> SendReportEmail();
        //  Task<IEnumerable<ReportSchedulerMap>> GetReportEmailDetails();
        // Task<int> UpdateTimeRangeByDate(ReportEmailFrequency reportEmailFrequency);
    }
}
