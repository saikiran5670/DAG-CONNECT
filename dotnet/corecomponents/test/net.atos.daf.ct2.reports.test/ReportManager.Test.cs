using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reports.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reports.test
{
    [TestClass]
    public class ReportManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly ReportRepository _reportRepository;
        private readonly IReportManager _reportManager;
        public ReportManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _reportRepository = new ReportRepository(_dataAccess);
            _reportManager = new ReportManager(_reportRepository);
        }

        #region Select User Preferences
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get User Preferences for a report success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetUserPreferences_Success()
        {            
            var result = await _reportManager.GetUserPreferenceReportDataColumn(1, 144);
            Assert.IsTrue(result.Count() > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get User Preferences for a report failure case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetUserPreferences_Failure()
        {
            var result = await _reportManager.GetUserPreferenceReportDataColumn(100000, 144);
            Assert.IsTrue(result.Count() == 0);
        }
        #endregion
    }
}
