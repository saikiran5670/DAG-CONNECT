using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public interface IReportSchedulerManager
    {
        Task<ReportParameter> GetReportParameter(int accountid, int organizationid);
        Task<ReportScheduler> CreateReportSchedular(ReportScheduler report);
        Task<ReportScheduler> UpdateReportSchedular(ReportScheduler report);
        Task<IEnumerable<ReportScheduler>> GetReportSchedulerList(int organizationid);
        Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel);
    }
}
