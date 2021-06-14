using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.rfms.responce;
using net.atos.daf.ct2.utilities;


namespace net.atos.daf.ct2.rfms.test
{
    [TestClass]
    public class rfmsmoqtest
    {

        Mock<IRfmsRepository> _iRfmsRepository;
        Mock<IAuditTraillib> _IAuditTraillib;
        RfmsManager _rfmsManager;
        public rfmsmoqtest()
        {
            _iRfmsRepository = new Mock<IRfmsRepository>();
            _IAuditTraillib = new Mock<IAuditTraillib>();
            _rfmsManager = new RfmsManager(_iRfmsRepository.Object, _IAuditTraillib.Object);
        }        
    
    [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVehicles")]
        [TestMethod]
        public async Task GetVehiclesTest()
        {            
            RfmsVehicleRequest rfmsVehicleRequest = new RfmsVehicleRequest();
           
           RfmsVehicles actual = new RfmsVehicles();
            _iRfmsRepository.Setup(s=>s.GetVehicles(It.IsAny<RfmsVehicleRequest>())).ReturnsAsync(actual);
            var result = await _rfmsManager.GetVehicles(rfmsVehicleRequest);
            Assert.AreEqual(result, actual);
        }
    }
}
