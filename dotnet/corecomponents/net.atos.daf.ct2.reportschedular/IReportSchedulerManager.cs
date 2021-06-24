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
        Task<ReportSchedulerMap> CreateReportScheduler(ReportSchedulerMap report);
        Task<ReportSchedulerMap> UpdateReportScheduler(ReportSchedulerMap report);
        Task<IEnumerable<ReportSchedulerMap>> GetReportSchedulerList(int organizationid);
        Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel);
        Task<List<PDFReportScreenModel>> GetPDFBinaryFormatById(ReportPDFByidModel request);
        Task<List<PDFReportScreenModel>> GetPDFBinaryFormatByToken(ReportPDFBytokenModel request);
    }
}
