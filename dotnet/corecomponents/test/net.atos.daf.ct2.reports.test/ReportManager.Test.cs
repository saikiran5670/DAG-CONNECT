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
        private readonly IDataMartDataAccess _dataMartdataAccess;
        private readonly ReportRepository _reportRepository;
        private readonly IReportManager _reportManager;
        public ReportManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            var connectionString = _config.GetConnectionString("DevAzure");

            _dataMartdataAccess = new PgSQLDataMartDataAccess(_config.GetConnectionString("DataMartConnectionString"));
            _dataAccess = new PgSQLDataAccess(connectionString);
            _reportRepository = new ReportRepository(_dataAccess, _dataMartdataAccess);
            _reportManager = new ReportManager(_reportRepository);
        }

        #region Select User Preferences
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get User Preferences for a report success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetUserPreferences_Success()
        {            
            var result = await _reportManager.GetUserPreferenceReportDataColumn(1, 144,1);
            Assert.IsTrue(result.Count() > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get User Preferences for a report failure case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetUserPreferences_Failure()
        {
            var result = await _reportManager.GetUserPreferenceReportDataColumn(100000, 144,1);
            Assert.IsTrue(result.Count() == 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get User Preferences for a report Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetRoleBasedDataColumn_Success()
        {
            var result = await _reportManager.GetRoleBasedDataColumn(1, 144, 1);
            Assert.IsTrue(result.Count() > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get User Preferences for a report failure case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetRoleBasedDataColumn_Falure()
        {
            var result = await _reportManager.GetRoleBasedDataColumn(10000, 144, 1);
            Assert.IsTrue(result.Count() == 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get GetVinsFromTripStatistics Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVinsFromTripStatistics_Success()
        {
            
            var result = await _reportManager
                                    .GetVinsFromTripStatistics(1604327461000, 1604336647000,
                                                               new List<string> {
                                                               "5A25561",
                                                               "ATOSGJ6237G001973",
                                                               "xxxxxxxx",
                                                               "ATOSGJ6237G784859"
                                                               });
            Assert.IsTrue(result.Count() == 2);
        }
        #endregion
    }
}
