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
    public class poimoqtest
    {
        Mock<IPoiRepository> _iPoiRepository;
        PoiManager _poiManager;
        public poimoqtest()
        {
            _iPoiRepository = new Mock<IPoiRepository>();
            _poiManager = new PoiManager(_iPoiRepository.Object);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAllGobalPOI")]
        [TestMethod]
        public async Task GetAllGobalPOITest()
        {            
            POIEntityRequest objPOIEntityRequest = new POIEntityRequest();
           // objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            List<POI> actual =new List<POI>();
            _iPoiRepository.Setup(s=>s.GetAllGobalPOI(It.IsAny<POIEntityRequest>())).ReturnsAsync(actual);
            var result = await _poiManager.GetAllGobalPOI(objPOIEntityRequest);
            Assert.AreEqual(result, actual);
        }    

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAllPOI")]
        [TestMethod]
        public async Task GetAllPOITest()
        {            
            POI poi = new POI();
           // objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            List<POI> actual =new List<POI>();
            _iPoiRepository.Setup(s=>s.GetAllPOI(It.IsAny<POI>())).ReturnsAsync(actual);
            var result = await _poiManager.GetAllPOI(poi);
            Assert.AreEqual(result, actual);
        }  
        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreatePOI")]
        [TestMethod]
        public async Task CreatePOITest()
        {            
            POI poi = new POI();
           // objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            POI actual =new POI();
            _iPoiRepository.Setup(s=>s.CreatePOI(It.IsAny<POI>())).ReturnsAsync(actual);
            var result = await _poiManager.CreatePOI(poi);
            Assert.AreEqual(result, actual);
        }        

         [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdatePOI")]
        [TestMethod]
        public async Task UpdatePOITest()
        {            
            POI poi = new POI();
           // objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            POI actual =new POI();
            _iPoiRepository.Setup(s=>s.UpdatePOI(It.IsAny<POI>())).ReturnsAsync(actual);
            var result = await _poiManager.UpdatePOI(poi);
            Assert.AreEqual(result, actual);
        }
         [TestCategory("Unit-Test-Case")]
        [Description("Test for DeletePOI")]
        [TestMethod]
        public async Task DeletePOITest()
        {            
        int poiId =43;
           // objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            //POI actual =new POI();
            _iPoiRepository.Setup(s=>s.DeletePOI(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _poiManager.DeletePOI(poiId);
            Assert.AreEqual(result, true);
        }
         [TestCategory("Unit-Test-Case")]
        [Description("Test for UploadPOI")]
        [TestMethod]
        public async Task UploadPOITest()
        {            
            UploadPOIExcel uploadPOIExcel = new UploadPOIExcel();
           // objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            UploadPOIExcel actual =new UploadPOIExcel();
            _iPoiRepository.Setup(s=>s.UploadPOI(It.IsAny<UploadPOIExcel>())).ReturnsAsync(actual);
            var result = await _poiManager.UploadPOI(uploadPOIExcel);
            Assert.AreEqual(result, actual);
        }
    
    }
}
