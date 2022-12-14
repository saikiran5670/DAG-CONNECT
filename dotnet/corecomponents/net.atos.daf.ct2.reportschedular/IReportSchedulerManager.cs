using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial interface IReportSchedulerManager
    {
        Task<ReportParameter> GetReportParameter(int accountid, int organizationid, int contextorgId, int roleId);
        Task<ReportSchedulerMap> CreateReportScheduler(ReportSchedulerMap report);
        Task<ReportSchedulerMap> UpdateReportScheduler(ReportSchedulerMap report);
        Task<IEnumerable<ReportSchedulerMap>> GetReportSchedulerList(int organizationid, List<int> vehicleIds, List<int> groupIds);
        Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel);
        Task<PDFReportScreenModel> GetPDFBinaryFormatById(ReportPDFByidModel request);
        Task<PDFReportScreenModel> GetPDFBinaryFormatByToken(ReportPDFBytokenModel request);
        Task<string> UpdatePDFBinaryRecordByToken(string token);
        Task<bool> UnSubscribeById(int recipientId, string emailId);
        Task<bool> UnSubscribeAllByEmailId(string emailId);
        Task<IEnumerable<ScheduledReport>> GetScheduledReport(int reportschedulerId);
    }
}
