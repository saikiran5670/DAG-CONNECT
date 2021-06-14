using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.repository;
using net.atos.daf.ct2.utilities;
namespace net.atos.daf.ct2.reports.test
{
    [TestClass]
    public class reportsmoqtest
    {

        Mock<IReportRepository> _iReportRepository;
        ReportManager _reportManager;
        public reportsmoqtest()
        {
            _iReportRepository = new Mock<IReportRepository>();
            _reportManager = new ReportManager(_iReportRepository.Object);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetUserPreferenceReportDataColumn")]
        [TestMethod]
        public async Task GetUserPreferenceReportDataColumnTest()
        {     

            int reportId = 23;
            int accountId = 35;
            int organizationId = 58;
            var actual =new List<UserPrefernceReportDataColumn>();
            _iReportRepository.Setup(s=>s.GetUserPreferenceReportDataColumn(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _reportManager.GetUserPreferenceReportDataColumn(reportId,accountId, organizationId);
            Assert.AreEqual(result, actual);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetRoleBasedDataColumn")]
        [TestMethod]
        public async Task GetRoleBasedDataColumnTest()
        {     

            int reportId = 23;
            int accountId = 35;
            int organizationId = 58;
            var actual =new List<UserPrefernceReportDataColumn>();
            _iReportRepository.Setup(s=>s.GetRoleBasedDataColumn(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _reportManager.GetRoleBasedDataColumn(reportId,accountId, organizationId);
            Assert.AreEqual(result, actual);
        }       
    

    }
}
