using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;


namespace net.atos.daf.ct2.reportscheduler.repository
{
    public interface IReportSchedulerRepository
    {
        #region Parameter Report Schedular
        Task<IEnumerable<ReportType>> GetReportType(int accountid, int organizationid);
        Task<IEnumerable<ReceiptEmails>> GetRecipientsEmails(int organizationid);
        Task<IEnumerable<DriverDetail>> GetDriverDetails(int organizationid);
        Task<ReportScheduler> CreateReportScheduler(ReportScheduler report);
        Task<ReportScheduler> UpdateReportScheduler(ReportScheduler report);
        Task<IEnumerable<ReportScheduler>> GetReportSchedulerList(int organizationid);
        Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel);
        #endregion
    }
}
