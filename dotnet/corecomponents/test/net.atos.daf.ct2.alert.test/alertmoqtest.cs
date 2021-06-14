using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.alert.test
{
    [TestClass]
    public class alertmoqtest
    {
         Mock<IAlertRepository> _iAlertRepository;
       AlertManager _alertManager;
        public alertmoqtest()
        {
            _iAlertRepository = new Mock<IAlertRepository>();
            _alertManager = new AlertManager(_iAlertRepository.Object);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for ActivateAlert")]
        [TestMethod]       
         public async Task ActivateAlertTest()
        {     

            int alertId = 32;
            char state = 'A';  
            char checkState = 'I';
            _iAlertRepository.Setup(s=>s.UpdateAlertState(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<char>())).ReturnsAsync(23);
            var result = await _alertManager.ActivateAlert(alertId,state,checkState);
            Assert.AreEqual(result, 23);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for SuspendAlert")]
        [TestMethod]   public async Task SuspendAlertTest()
        {     

            int alertId = 32;
            char state = 'A';  
            char checkState = 'I';
            _iAlertRepository.Setup(s=>s.UpdateAlertState(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<char>())).ReturnsAsync(23);
            var result = await _alertManager.SuspendAlert(alertId,state,checkState);
            Assert.AreEqual(result, 23);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteAlert")]
        [TestMethod]   public async Task DeleteAlertTest()
        {     

            int alertId = 32;
            char state = 'A';  
            _iAlertRepository.Setup(s=>s.AlertStateToDelete(It.IsAny<int>(), It.IsAny<char>())).ReturnsAsync(23);
            var result = await _alertManager.DeleteAlert(alertId,state);
            Assert.AreEqual(result, 23);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for CheckIsNotificationExitForAlert")]
        [TestMethod]   public async Task CheckIsNotificationExitForAlertTest()
        {     

            int alertId = 32;
            _iAlertRepository.Setup(s=>s.CheckIsNotificationExitForAlert(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _alertManager.CheckIsNotificationExitForAlert(alertId);
            Assert.AreEqual(result, true);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreateAlertTest")]
        [TestMethod]   public async Task CreateAlertTest()
        {     
            Alert alert = new Alert();

              alert.OrganizationId = 10;
                alert.Name = "TestAlert1";
                alert.Category = "L";
                alert.Type = "N";
                alert.ValidityPeriodType = "A";
                 Alert actual =new Alert();
            _iAlertRepository.Setup(s=>s.CreateAlert(It.IsAny<Alert>())).ReturnsAsync(actual);
            var result = await _alertManager.CreateAlert(alert);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateAlert")]
        [TestMethod]   public async Task UpdateAlertTest()
        {     
            Alert alert = new Alert();

              alert.OrganizationId = 10;
                alert.Name = "TestAlert1";
                alert.Category = "L";
                alert.Type = "N";
                alert.ValidityPeriodType = "A";
                 Alert actual =new Alert();
            _iAlertRepository.Setup(s=>s.UpdateAlert(It.IsAny<Alert>())).ReturnsAsync(actual);
            var result = await _alertManager.UpdateAlert(alert);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAlertList")]
        [TestMethod]   public async Task GetAlertListTest()
        {     

            int accountid = 32;
            int organizationid = 26; 
            var actual = new List<Alert>();
            _iAlertRepository.Setup(s=>s.GetAlertList(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _alertManager.GetAlertList(accountid,organizationid);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for DuplicateAlertType")]
        [TestMethod]   public async Task DuplicateAlertTypeTest()
        {     

            int alertId = 32; 
            DuplicateAlertType actual =new DuplicateAlertType();
            _iAlertRepository.Setup(s=>s.DuplicateAlertType(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _alertManager.DuplicateAlertType(alertId);
            Assert.AreEqual(result, actual);
        }
    }
}
