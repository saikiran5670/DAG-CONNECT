using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.role.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.role.test
{
    [TestClass]
    public class rolemoqtest
    {

         Mock<IRoleRepository> _iRoleRepository;
         Mock<IFeatureRepository> _iFeatureRepository;
         Mock<IFeatureManager> _iFeatureManager;

        RoleManagement _IRoleManagement;
        public rolemoqtest()
        {
            _iRoleRepository = new Mock<IRoleRepository>();
            _iFeatureRepository = new Mock<IFeatureRepository>();
            _iFeatureManager = new Mock<IFeatureManager>();
            _IRoleManagement = new RoleManagement(_iRoleRepository.Object, _iFeatureManager.Object, _iFeatureRepository.Object);
        }    

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create")]
        [TestMethod]
        public async Task CreateRoleTest()
        {     

              
           RoleMaster roleMaster = new RoleMaster();
            FeatureSet featureSet= new FeatureSet();
           List<Feature> features = new List<Feature>();

           
            roleMaster.FeatureSet = featureSet;
            roleMaster.FeatureSet.Features = features;
            _iFeatureManager.Setup(x=>x.AddFeatureSet(It.IsAny<FeatureSet>())).ReturnsAsync(1);
            _iFeatureManager.Setup(b=>b.GetMinimumLevel(It.IsAny<List<Feature>>())).ReturnsAsync(3);
            _iRoleRepository.Setup(s=>s.CreateRole(It.IsAny<RoleMaster>())).ReturnsAsync(12);
            var result = await _IRoleManagement.CreateRole(roleMaster);
            Assert.AreEqual(result, 12);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete")]
        [TestMethod]
        public async Task DeleteRoleTest()
        {     

              
             int roleid = 1;
            int accountid = 20;
            _iRoleRepository.Setup(s=>s.DeleteRole(It.IsAny<int>(),It.IsAny<int>())).ReturnsAsync(12);
            var result = await _IRoleManagement.DeleteRole(roleid, accountid);
            Assert.AreEqual(result, 12);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update")]
        [TestMethod]
        public async Task UpdateRoleTest_WithFeatureSetIdAsZero()
        {     

        
           RoleMaster roleMaster = new RoleMaster();
           FeatureSet featureSet= new FeatureSet();
           List<Feature> features = new List<Feature>();
           roleMaster.Name = "UpdateRole";
            roleMaster.Id = 5;
            roleMaster.Updatedby = 6;
            roleMaster.FeatureSet = featureSet;
             roleMaster.FeatureSet.Features = features;

            _iFeatureRepository.Setup(x=>x.AddFeatureSet(It.IsAny<FeatureSet>())).ReturnsAsync(1);
            _iFeatureRepository.Setup(b=>b.GetMinimumLevel(It.IsAny<List<Feature>>())).ReturnsAsync(3);
            _iRoleRepository.Setup(s=>s.UpdateRole(It.IsAny<RoleMaster>())).ReturnsAsync(12);
            var result = await _IRoleManagement.UpdateRole(roleMaster);
            Assert.AreEqual(result, 0);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update")]
        [TestMethod]
        public async Task UpdateRoleTest_WithFeatureSetId()
        {     

        
           RoleMaster roleMaster = new RoleMaster();
           FeatureSet featureSet= new FeatureSet();
           featureSet.FeatureSetID=10;
           List<Feature> features = new List<Feature>();
           roleMaster.Name = "UpdateRole";
            roleMaster.Id = 5;
            roleMaster.Updatedby = 6;
            roleMaster.FeatureSet = featureSet;
             roleMaster.FeatureSet.Features = features;

            _iFeatureManager.Setup(x=>x.AddFeatureSet(It.IsAny<FeatureSet>())).ReturnsAsync(1);
            _iFeatureManager.Setup(b=>b.GetMinimumLevel(It.IsAny<List<Feature>>())).ReturnsAsync(3);
            _iRoleRepository.Setup(s=>s.UpdateRole(It.IsAny<RoleMaster>())).ReturnsAsync(12);
            var result = await _IRoleManagement.UpdateRole(roleMaster);
            Assert.AreEqual(result, 12);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get")]
        [TestMethod]
        public async Task GetRoleTest()
        {     

              
           RoleFilter rolefilter = new RoleFilter();


            rolefilter.Organization_Id = 12;

           // int GetFeatureIdsForFeatureSet=3;
            // string Langaugecode="3";
            var actual = new List<RoleMaster>();
            var some = new List<Feature>();

           // _iFeatureManager.Setup(x=>x.GetFeatureIdsForFeatureSet(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);
            _iRoleRepository.Setup(s=>s.GetRoles(It.IsAny<RoleFilter>())).ReturnsAsync(actual);
            var result = await _IRoleManagement.GetRoles(rolefilter);
            Assert.AreEqual(result, actual);
        }




    }
}
