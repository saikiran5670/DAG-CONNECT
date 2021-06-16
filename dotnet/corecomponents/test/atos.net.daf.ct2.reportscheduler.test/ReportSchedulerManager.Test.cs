using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportscheduler;
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
        public ReportSchedulerManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                       .Build();
            var connectionString = _config.GetConnectionString("DevAzure");

            _dataMartdataAccess = new PgSQLDataMartDataAccess(_config.GetConnectionString("DataMartConnectionString"));
            _dataAccess = new PgSQLDataAccess(connectionString);
            _reportSchedulerRepository = new ReportSchedulerRepository(_dataAccess, _dataMartdataAccess);
            _reportSchedulerManager = new ReportSchedulerManager(_reportSchedulerRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get report type")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_ReportScheduler_GetReportParameter()
        {
            int AccountId = 393;
            int OrgnisationId = 1;
            var result = await _reportSchedulerManager.GetReportParameter(AccountId, OrgnisationId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }
    }
}
