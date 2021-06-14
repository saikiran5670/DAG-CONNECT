using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class corridormoqtest
    {
         Mock<ICorridorRepository> _iCorridorRepository;
        CorridorManger _corridorManager;
        public corridormoqtest()
        {
            _iCorridorRepository = new Mock<ICorridorRepository>();
            _corridorManager = new CorridorManger(_iCorridorRepository.Object);
        }  
        [TestCategory("Unit-Test-Case")]
        [Description("Test for AddExistingTripCorridor")]
        [TestMethod]
        public async Task AddExistingTripCorridorTest()
        {            
            ExistingTripCorridor existingTripCorridor = new ExistingTripCorridor();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            ExistingTripCorridor actual =new ExistingTripCorridor();
            _iCorridorRepository.Setup(s=>s.AddExistingTripCorridor(It.IsAny<ExistingTripCorridor>())).ReturnsAsync(actual);
            var result = await _corridorManager.AddExistingTripCorridor(existingTripCorridor);
            Assert.AreEqual(result, actual);
        }      
        [TestCategory("Unit-Test-Case")]
        [Description("Test for AddRouteCorridor")]
        [TestMethod]
        public async Task AddRouteCorridorTest()
        {            
            RouteCorridor routeCorridor = new RouteCorridor();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            RouteCorridor actual =new RouteCorridor();
            _iCorridorRepository.Setup(s=>s.AddRouteCorridor(It.IsAny<RouteCorridor>())).ReturnsAsync(actual);
            var result = await _corridorManager.AddRouteCorridor(routeCorridor);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteCorridor")]
        [TestMethod]
        public async Task DeleteCorridorTest()
        {            
           int CorridorId= 54;
            CorridorID actual =new CorridorID();
            _iCorridorRepository.Setup(s=>s.DeleteCorridor(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _corridorManager.DeleteCorridor(CorridorId);
            Assert.AreEqual(result, actual);
        }  
        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetCorridorList")]
        [TestMethod]
        public async Task GetCorridorListTest()
        {            
            CorridorRequest objCorridorRequest = new CorridorRequest();
            
            CorridorLookUp actual =new CorridorLookUp();
            // List<ViaAddressDetail> address = new List<ViaAddressDetail>(){new ViaAddressDetail(){Latitude =100 }};
            // _iCorridorRepository.Setup(i=>i.GetCorridorListByOrgIdAndCorriId()).Ret
            //_iCorridorRepository.Setup(s=>s.GetCorridorList(It.IsAny<CorridorRequest>())).ReturnsAsync(actual);
            var result = await _corridorManager.GetCorridorList(objCorridorRequest);
            Assert.AreEqual(result, actual);
        }  
         [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateExistingTripCorridor")]
        [TestMethod]        
        public async Task UpdateExistingTripCorridorTest()
        {            
            ExistingTripCorridor existingTripCorridor = new ExistingTripCorridor();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            ExistingTripCorridor actual =new ExistingTripCorridor();
            _iCorridorRepository.Setup(s=>s.UpdateExistingTripCorridor(It.IsAny<ExistingTripCorridor>())).ReturnsAsync(actual);
            var result = await _corridorManager.UpdateExistingTripCorridor(existingTripCorridor);
            Assert.AreEqual(result, actual);
        }  
        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateRouteCorridor")]
        [TestMethod]        
        public async Task UpdateRouteCorridorTest()
        {            
            RouteCorridor objRouteCorridor = new RouteCorridor();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            RouteCorridor actual =new RouteCorridor();
            _iCorridorRepository.Setup(s=>s.UpdateRouteCorridor(It.IsAny<RouteCorridor>())).ReturnsAsync(actual);
            var result = await _corridorManager.UpdateRouteCorridor(objRouteCorridor);
            Assert.AreEqual(result, actual);
        } 
    }
}
