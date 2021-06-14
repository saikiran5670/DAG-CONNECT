using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.relationship.entity;
using net.atos.daf.ct2.relationship.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.relationship.test
{
    [TestClass]
    public class relationshipmoqtest
    {
         Mock<IRelationshipRepository> _iRelationshipRepository;
        RelationshipManager _relationshipManager;
        public relationshipmoqtest()
        {
            _iRelationshipRepository = new Mock<IRelationshipRepository>();
            _relationshipManager = new RelationshipManager(_iRelationshipRepository.Object);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreateRelationship")]
        [TestMethod]
        public async Task CreateRelationshipTest()
        {            
            Relationship orgRelationship = new Relationship();
             orgRelationship.OrganizationId = 1;
            orgRelationship.Code = "C1";
            orgRelationship.Level = 1;
            orgRelationship.Name = "Test Data";
            orgRelationship.Description = "Unit testing";
            orgRelationship.FeaturesetId = 1;
            orgRelationship.State = "A";
            Relationship actual =new Relationship();
            _iRelationshipRepository.Setup(s=>s.CreateRelationship(It.IsAny<Relationship>())).ReturnsAsync(actual);
            var result = await _relationshipManager.CreateRelationship(orgRelationship);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateRelationship")]
        [TestMethod]
        public async Task UpdateRelationshipTest()
        {            
            Relationship orgRelationship = new Relationship();
             orgRelationship.OrganizationId = 1;
            orgRelationship.Code = "C1";
            orgRelationship.Level = 1;
            orgRelationship.Name = "Test Data";
            orgRelationship.Description = "Unit testing";
            orgRelationship.FeaturesetId = 1;
            orgRelationship.State = "A";
            Relationship actual =new Relationship();
            _iRelationshipRepository.Setup(s=>s.UpdateRelationship(It.IsAny<Relationship>())).ReturnsAsync(actual);
            var result = await _relationshipManager.UpdateRelationship(orgRelationship);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteRelationship")]
        [TestMethod]
        public async Task DeleteRelationshipTest()
        {            
            int orgRelationshipId = 34;
            //Relationship actual =new Relationship();
            _iRelationshipRepository.Setup(s=>s.DeleteRelationship(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _relationshipManager.DeleteRelationship(orgRelationshipId);
            Assert.AreEqual(result, true);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetRelationship")]
        [TestMethod]
        public async Task GetRelationshipTest()
        {            
            RelationshipFilter orgRelationship = new RelationshipFilter();
             orgRelationship.OrganizationId = 1;
            orgRelationship.Code = "C1";
            orgRelationship.Level = 1;
          //  orgRelationship.Name = "Test Data";
          //  orgRelationship.Description = "Unit testing";
            orgRelationship.FeaturesetId = 1;
           // orgRelationship.State = "A";
            List<Relationship> actual =new List<Relationship>();
            _iRelationshipRepository.Setup(s=>s.GetRelationship(It.IsAny<RelationshipFilter>())).ReturnsAsync(actual);
            var result = await _relationshipManager.GetRelationship(orgRelationship);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetRelationshipMapping")]
        [TestMethod]
        public async Task GetRelationshipMappingTest()
        {            
            OrganizationRelationShip orgMapfilter = new OrganizationRelationShip();
            // orgMapfilter.OrganizationId = 1;
           // orgRelationship.Code = "C1";
           // orgRelationship.Level = 1;
          //  orgRelationship.Name = "Test Data";
          //  orgRelationship.Description = "Unit testing";
           // orgRelationship.FeaturesetId = 1;
           // orgRelationship.State = "A";
            List<OrganizationRelationShip> actual =new List<OrganizationRelationShip>();
            _iRelationshipRepository.Setup(s=>s.GetRelationshipMapping(It.IsAny<OrganizationRelationShip>())).ReturnsAsync(actual);
            var result = await _relationshipManager.GetRelationshipMapping(orgMapfilter);
            Assert.AreEqual(result, actual);
        }


    }

}
