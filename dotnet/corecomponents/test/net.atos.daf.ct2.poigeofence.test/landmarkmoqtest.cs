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
    public class landmarkmoqtest
    {
        Mock<ILandmarkgroupRepository> _iLandmarkgroupRepository;
        LandmarkGroupManager _LandmarkGroupManager;
        public landmarkmoqtest()
        {
            _iLandmarkgroupRepository = new Mock<ILandmarkgroupRepository>();
            _LandmarkGroupManager = new LandmarkGroupManager(_iLandmarkgroupRepository.Object);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreateGroup")]
        [TestMethod]
        public async Task CreateGroupTest()
        {            
            LandmarkGroup landmarkgroup = new LandmarkGroup();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
            LandmarkGroup actual = new LandmarkGroup();
           
            _iLandmarkgroupRepository.Setup(s=>s.CreateGroup(It.IsAny<LandmarkGroup>())).ReturnsAsync(actual);
            var result = await _LandmarkGroupManager.CreateGroup(landmarkgroup);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateGroup")]
        [TestMethod]
        public async Task UpdateGroupTest()
        {            
            LandmarkGroup landmarkgroup = new LandmarkGroup();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
            LandmarkGroup actual = new LandmarkGroup();
           
            _iLandmarkgroupRepository.Setup(s=>s.UpdateGroup(It.IsAny<LandmarkGroup>())).ReturnsAsync(actual);
            var result = await _LandmarkGroupManager.UpdateGroup(landmarkgroup);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteGroup")]
        [TestMethod]
        public async Task DeleteGroupTest()
        {  
            int groupid=34;
             int modifiedby=23;          
            //LandmarkGroup landmarkgroup = new LandmarkGroup();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
            LandmarkGroup actual = new LandmarkGroup();
           
            _iLandmarkgroupRepository.Setup(s=>s.DeleteGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(23);
            var result = await _LandmarkGroupManager.DeleteGroup(groupid, modifiedby);
            Assert.AreEqual(result, 23);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetlandmarkGroup")]
        [TestMethod]
        public async Task GetlandmarkGroupTest()
        {  
            int organizationid=34;
             int groupid=23;          
            //LandmarkGroup landmarkgroup = new LandmarkGroup();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
            var actual = new List<LandmarkGroup>();
           
            _iLandmarkgroupRepository.Setup(s=>s.GetlandmarkGroup(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _LandmarkGroupManager.GetlandmarkGroup(groupid, organizationid);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Exists")]
        [TestMethod]
        public async Task Existsest()
        {            
            LandmarkGroup landmarkgroup = new LandmarkGroup();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // LandmarkGroup actual = new LandmarkGroup();
           
            _iLandmarkgroupRepository.Setup(s=>s.Exists(It.IsAny<LandmarkGroup>())).ReturnsAsync(23);
            var result = await _LandmarkGroupManager.Exists(landmarkgroup);
            Assert.AreEqual(result, 23);
        }
    }
}
