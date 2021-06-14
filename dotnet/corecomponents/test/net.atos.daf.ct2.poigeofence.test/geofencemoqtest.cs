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
    public class geofencemoqtest
    {
        Mock<IGeofenceRepository> _iGeofenceRepository;
        GeofenceManager _geofenceManager;
        public geofencemoqtest()
        {
            _iGeofenceRepository = new Mock<IGeofenceRepository>();
            _geofenceManager = new GeofenceManager(_iGeofenceRepository.Object);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteGeofence")]
        [TestMethod]
        public async Task DeleteGeofenceTest()
        {            
            GeofenceDeleteEntity objGeofenceDeleteEntity = new GeofenceDeleteEntity();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
           // List<SubscriptionDetails> actual =new List<SubscriptionDetails>();
            _iGeofenceRepository.Setup(s=>s.DeleteGeofence(It.IsAny<GeofenceDeleteEntity>())).ReturnsAsync(true);
            var result = await _geofenceManager.DeleteGeofence(objGeofenceDeleteEntity);
            Assert.AreEqual(result, true);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreatePolygonGeofence")]
        [TestMethod]
        public async Task CreatePolygonGeofenceTest()
        {            
            Geofence geofence = new Geofence();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            Geofence actual =new Geofence();
            _iGeofenceRepository.Setup(s=>s.CreatePolygonGeofence(It.IsAny<Geofence>(), It.IsAny<bool>())).ReturnsAsync(actual);
            var result = await _geofenceManager.CreatePolygonGeofence(geofence);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAllGeofence")]
        [TestMethod]
        public async Task GetAllGeofenceTest()
        {            
            GeofenceEntityRequest geofenceEntityRequest = new GeofenceEntityRequest();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            var actual =new List<GeofenceEntityResponce>();
            _iGeofenceRepository.Setup(s=>s.GetAllGeofence(It.IsAny<GeofenceEntityRequest>())).ReturnsAsync(actual);
            var result = await _geofenceManager.GetAllGeofence(geofenceEntityRequest);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreateCircularGeofence")]
        [TestMethod]
        public async Task CreateCircularGeofenceTest()
        {            
            List<Geofence> geofence = new List<Geofence>();
            
            List<Geofence> actual =new List<Geofence>();
            _iGeofenceRepository.Setup(s=>s.CreateCircularGeofence(It.IsAny<List<Geofence>>(), It.IsAny<bool>())).ReturnsAsync(actual);
            var result = await _geofenceManager.CreateCircularGeofence(geofence);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdatePolygonGeofence")]
        [TestMethod]
        public async Task UpdatePolygonGeofenceTest()
        {            
            Geofence geofence = new Geofence();
          
            Geofence actual =new Geofence();
            _iGeofenceRepository.Setup(s=>s.UpdatePolygonGeofence(It.IsAny<Geofence>())).ReturnsAsync(actual);
            var result = await _geofenceManager.UpdatePolygonGeofence(geofence);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetGeofenceByGeofenceID")]
        [TestMethod]
        public async Task GetGeofenceByGeofenceIDTest()
        {            
            int organizationId =23;
             int geofenceId = 32;
          
            var actual =new List<Geofence>();
            _iGeofenceRepository.Setup(s=>s.GetGeofenceByGeofenceID(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _geofenceManager.GetGeofenceByGeofenceID(organizationId, geofenceId);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAllGeofence")]
        [TestMethod]
        public async Task GetAllGeofencTest()
        {            
            Geofence geofenceFilter = new Geofence();
           
            var actual =new List<Geofence>();
            _iGeofenceRepository.Setup(s=>s.GetAllGeofence(It.IsAny<Geofence>())).ReturnsAsync(actual);
            var result = await _geofenceManager.GetAllGeofence(geofenceFilter);
            Assert.AreEqual(result, actual);
        }



    }
}
