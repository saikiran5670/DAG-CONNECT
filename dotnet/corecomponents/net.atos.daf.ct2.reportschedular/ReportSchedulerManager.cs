using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {
        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        public ReportSchedulerManager(IReportSchedulerRepository reportSchedularRepository)
        {
            _reportSchedulerRepository = reportSchedularRepository;
        }
        public async Task<ReportParameter> GetReportParameter(int accountid, int organizationid, int contextorgId, int roleId)
        {
            ReportParameter reportparameter = new ReportParameter();
            reportparameter.ReportType = await _reportSchedulerRepository.GetReportType(accountid, organizationid, contextorgId, roleId);
            reportparameter.DriverDetail = await _reportSchedulerRepository.GetDriverDetails(organizationid);
            reportparameter.ReceiptEmails = await _reportSchedulerRepository.GetRecipientsEmails(organizationid);
            return reportparameter;
        }

        public async Task<ReportSchedulerMap> CreateReportScheduler(ReportSchedulerMap report)
        {
            return await _reportSchedulerRepository.CreateReportScheduler(report);
        }

        public async Task<ReportSchedulerMap> UpdateReportScheduler(ReportSchedulerMap report)
        {
            return await _reportSchedulerRepository.UpdateReportScheduler(report);
        }

        public async Task<IEnumerable<ReportSchedulerMap>> GetReportSchedulerList(int organizationid)
        {
            return await _reportSchedulerRepository.GetReportSchedulerList(organizationid);
        }

        public async Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel)
        {
            return await _reportSchedulerRepository.ManipulateReportSchedular(objReportStatusUpdateDeleteModel);
        }

        public async Task<PDFReportScreenModel> GetPDFBinaryFormatById(ReportPDFByidModel request)
        {
            return await _reportSchedulerRepository.GetPDFBinaryFormatById(request);
        }
        public async Task<PDFReportScreenModel> GetPDFBinaryFormatByToken(ReportPDFBytokenModel request)
        {
            return await _reportSchedulerRepository.GetPDFBinaryFormatByToken(request);
        }
        public async Task<string> UpdatePDFBinaryRecordByToken(string token)
        {
            return await _reportSchedulerRepository.UpdatePDFBinaryRecordByToken(token);
        }
    }
}
