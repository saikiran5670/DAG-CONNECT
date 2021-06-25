﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.translation;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {
        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        private readonly IAccountManager _accountManager;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IConfiguration _configuration;
        readonly IAuditTraillib _auditlog;
        public ReportSchedulerManager(IReportSchedulerRepository reportSchedularRepository, IAuditTraillib auditlog, IAccountManager accountManager, IConfiguration configuration)
        {
            _reportSchedulerRepository = reportSchedularRepository;
            this._auditlog = auditlog;
            _accountManager = accountManager;
            this._configuration = configuration;
            _emailConfiguration = new EmailConfiguration();
            configuration.GetSection("EmailConfiguration").Bind(_emailConfiguration);
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

        public async Task<List<PDFReportScreenModel>> GetPDFBinaryFormatById(ReportPDFByidModel request)
        {
            return await _reportSchedulerRepository.GetPDFBinaryFormatById(request);
        }
        public async Task<List<PDFReportScreenModel>> GetPDFBinaryFormatByToken(ReportPDFBytokenModel request)
        {
            return await _reportSchedulerRepository.GetPDFBinaryFormatByToken(request);
        }
    }
}
