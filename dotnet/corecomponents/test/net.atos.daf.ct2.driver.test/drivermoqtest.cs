using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.driver.entity;
using net.atos.daf.ct2.driver;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.driver.test
{
  [TestClass]
    public class drivermoqtest
    {
        Mock<IDriverRepository> _iDriverRepository;

        Mock<IAuditTraillib> _auditlog;
        DriverManager _driverManager;
        public drivermoqtest()
        {
            _iDriverRepository = new Mock<IDriverRepository>();
            _auditlog = new Mock<IAuditTraillib>();
            _driverManager = new DriverManager(_iDriverRepository.Object, _auditlog.Object);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for ImportDrivers")]
        [TestMethod]
        public async Task ImportDriversTest()
        {     

           List<Driver> driver = new List<Driver>();
            int orgid = 34;      
          
            List<DriverImportResponse> actual =new List<DriverImportResponse>();
            _iDriverRepository.Setup(s=>s.ImportDrivers(It.IsAny<List<Driver>>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _driverManager.ImportDrivers(driver,orgid);
            Assert.AreEqual(result, actual);
        }  
        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetDriver")]
        [TestMethod]
        public async Task GetDriverTest()
        {     

          int OrganizationId =23;
            int DriverID = 34;      
          
            var actual =new List<DriverResponse>();
            _iDriverRepository.Setup(s=>s.GetDriver(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _driverManager.GetDriver(DriverID,OrganizationId);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateDriver")]
        [TestMethod]
        public async Task UpdateDriverTest()
        {     

           Driver driver = new Driver();
               
          
            Driver actual = new Driver();
            _iDriverRepository.Setup(s=>s.UpdateDriver(It.IsAny<Driver>())).ReturnsAsync(actual);
            var result = await _driverManager.UpdateDriver(driver);
            Assert.AreEqual(result, actual);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteDriver")]
        [TestMethod]
        public async Task DeleteDriverTest()
        {     

          int OrganizationId =23;
            int DriverID = 34;      
          
           // var actual =new List<DriverResponse>();
            _iDriverRepository.Setup(s=>s.DeleteDriver(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            var result = await _driverManager.DeleteDriver(DriverID,OrganizationId);
            Assert.AreEqual(result, true);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateOptinOptout")]
        [TestMethod]
        public async Task UpdateOptinOptoutTest()
        {     

          int OrganizationId =23;
            string optoutStatus = "wtgjkj";    
          
           // var actual =new List<DriverResponse>();
            _iDriverRepository.Setup(s=>s.UpdateOptinOptout(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await _driverManager.UpdateOptinOptout(OrganizationId,optoutStatus);
            Assert.AreEqual(result, true);
        }


    }
}
