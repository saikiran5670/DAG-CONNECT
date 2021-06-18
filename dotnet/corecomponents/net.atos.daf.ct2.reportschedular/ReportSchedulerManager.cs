using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public class ReportSchedulerManager : IReportSchedulerManager
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

        public async Task<ReportScheduler> CreateReportSchedular(ReportScheduler report)
        {
            return await _reportSchedulerRepository.CreateReportSchedular(report);
        }

        public async Task<ReportScheduler> UpdateReportSchedular(ReportScheduler report)
        {
            return await _reportSchedulerRepository.CreateReportSchedular(report);
        }

        public async Task<IEnumerable<ReportScheduler>> GetReportSchedulerList(int organizationid)
        {
            return await _reportSchedulerRepository.GetReportSchedulerList(organizationid);
        }

        public async Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel)
        {
            return await _reportSchedulerRepository.ManipulateReportSchedular(objReportStatusUpdateDeleteModel);
        }
    }
}
