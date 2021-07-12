using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.ENUM;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.utilities;

namespace atos.net.daf.ct2.reportscheduler.test
{
    [TestClass]
    public class ReportSchedulerManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly ReportSchedulerRepository _reportSchedulerRepository;
        private readonly IReportSchedulerManager _reportSchedulerManager;
        private readonly IEmailNotificationManager _emailNotificationManager;
        readonly IAuditTraillib _auditlog;
        private readonly Helper _helper;
        public ReportSchedulerManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                       .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _reportSchedulerRepository = new ReportSchedulerRepository(_dataAccess);
            _reportSchedulerManager = new ReportSchedulerManager(_reportSchedulerRepository);
            _helper = new Helper();
        }

        [TestMethod]
        public async Task GetReportEmailDetailsTest()
        {
            var result = await _reportSchedulerRepository.GetReportEmailDetails();
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get report type")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_ReportScheduler_GetReportParameter()
        {
            int accountId = 393;
            int orgnisationId = 1;
            int contextorgId = 36;
            int roleId = 33;
            var result = await _reportSchedulerManager.GetReportParameter(accountId, orgnisationId, contextorgId, roleId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for create report scheduler")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_ReportScheduler_CreateReportScheduler()
        {
            ReportSchedulerMap report = new ReportSchedulerMap();
            var result = await _reportSchedulerManager.CreateReportScheduler(report);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update report scheduler")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_ReportScheduler_UpdateReportScheduler()
        {
            ReportSchedulerMap report = new ReportSchedulerMap();
            var result = await _reportSchedulerManager.UpdateReportScheduler(report);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for get report scheduler")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_ReportScheduler_GetReportSchedulerList()
        {
            int orgnisationId = 1;
            var result = await _reportSchedulerManager.GetReportSchedulerList(orgnisationId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for manipulate report scheduler")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task Unt_ReportSchedular_ManipulateReportSchedular()
        {
            ReportStatusUpdateDeleteModel reportStatusUpdateDeleteModel = new ReportStatusUpdateDeleteModel();
            var result = await _reportSchedulerManager.ManipulateReportSchedular(reportStatusUpdateDeleteModel);
            Assert.IsNotNull(result);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get PDFBinary Format By Id")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]

        public async Task Unt_ReportSchedular_GetPDFBinaryFormatById()
        {
            ReportPDFByidModel reportPDFByidModel = new ReportPDFByidModel();
            var result = await _reportSchedulerManager.GetPDFBinaryFormatById(reportPDFByidModel);
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get PDFBinary format by Token")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task Unt()
        {
            ReportPDFBytokenModel reportPDFBytokenModel = new ReportPDFBytokenModel();
            var result = await _reportSchedulerManager.GetPDFBinaryFormatByToken(reportPDFBytokenModel);
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for time frequency type")]
        [TestMethod]
        public async Task UnT_Helpr_GetNextFrequencyTime()
        {
            long currentdate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            bool isresult;
            ReportEmailFrequency objReportEmailFrequency = new ReportEmailFrequency();
            objReportEmailFrequency.ReportNextScheduleRunDate = 1625574329670;
            objReportEmailFrequency.FrequencyType = TimeFrequenyType.Daily;
            _helper.GetNextFrequencyTime(objReportEmailFrequency);
            isresult = true;
            Assert.IsTrue(isresult);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for time Quarter time")]
        [TestMethod]
        public async Task UnT_Helpr_GetNextQuarterTime()
        {
            long date = 1624184687000;
            var result = _helper.GetNextQuarterTime(date);
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for time Monthly Time")]
        [TestMethod]
        public async Task UnT_Helpr_GetNextMonthlyTime()
        {
            long date = 1624184687000;
            var result = _helper.GetNextMonthlyTime(date);
            Assert.IsNotNull(result);
        }
    }
}
