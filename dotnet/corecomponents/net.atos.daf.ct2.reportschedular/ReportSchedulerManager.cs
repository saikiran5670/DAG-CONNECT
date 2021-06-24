using System;
using System.Collections.Generic;
using System.Text;
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
        public async Task<ReportParameter> GetReportParameter(int accountid, int organizationid)
        {
            ReportParameter reportparameter = new ReportParameter();
            reportparameter.ReportType = await _reportSchedulerRepository.GetReportType(accountid, organizationid);
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

        public async Task<List<PDFReportScreenModel>> GetPDFBinaryFormatById(int reportId)
        {
            return await _reportSchedulerRepository.GetPDFBinaryFormatById(reportId);
        }
        public async Task<List<PDFReportScreenModel>> GetPDFBinaryFormatByToken(string token)
        {
            return await _reportSchedulerRepository.GetPDFBinaryFormatByToken(token);
        }
    }
}
