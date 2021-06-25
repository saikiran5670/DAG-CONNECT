using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.ENUM;
using net.atos.daf.ct2.reportscheduler.repository;

namespace atos.net.daf.ct2.reportscheduler.test
{
    [TestClass]
    public class ReportSchedulerManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        private readonly ReportSchedulerRepository _reportSchedulerRepository;
        private readonly IReportSchedulerManager _reportSchedulerManager;
        private readonly Helper _helper;
        public ReportSchedulerManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                       .Build();
            var connectionString = _config.GetConnectionString("DevAzure");

            _dataMartdataAccess = new PgSQLDataMartDataAccess(_config.GetConnectionString("DataMartConnectionString"));
            _dataAccess = new PgSQLDataAccess(connectionString);
            _reportSchedulerRepository = new ReportSchedulerRepository(_dataAccess, _dataMartdataAccess);
            _reportSchedulerManager = new ReportSchedulerManager(_reportSchedulerRepository);
            _helper = new Helper();
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get report type")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_ReportScheduler_GetReportParameter()
        {
            int accountId = 393;
            int orgnisationId = 1;
            var result = await _reportSchedulerManager.GetReportParameter(accountId, orgnisationId);
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
            long date = 1624184687000;
            TimeFrequenyType timeFrequeny = TimeFrequenyType.Daily;
            var result = _helper.GetNextFrequencyTime(date, timeFrequeny);
            Assert.IsNotNull(result);
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
