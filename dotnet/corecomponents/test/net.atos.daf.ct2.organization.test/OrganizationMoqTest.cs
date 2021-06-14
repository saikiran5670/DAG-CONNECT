using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.utilities;


namespace net.atos.daf.ct2.organization.test
{
    [TestClass]
    public class OrganizationMoqTest
    {
        Mock<IOrganizationRepository> _iOrganizationRepository;
        Mock<IAuditTraillib> _auditlog;
        OrganizationManager _OrganizationManager;
        public OrganizationMoqTest()
        {
            _iOrganizationRepository = new Mock<IOrganizationRepository>();
            _auditlog = new Mock<IAuditTraillib>();

            _OrganizationManager = new OrganizationManager(_iOrganizationRepository.Object, _auditlog.Object);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create")]
        [TestMethod]
        public async Task CreateTest()
        {            
            Organization organization = new Organization();
           organization.OrganizationId = "Test";
            organization.Type = "";
            organization.Name = "TestOrg";
            organization.AddressType = "1";
            organization.AddressStreet = null;
            organization.AddressStreetNumber = null;
            organization.PostalCode = null;
            organization.City = null;
            organization.CountryCode = null;
            Organization actual =new Organization();
            _iOrganizationRepository.Setup(s=>s.Create(It.IsAny<Organization>())).ReturnsAsync(actual);
            var result = await _OrganizationManager.Create(organization);
            Assert.AreEqual(result, actual);
        }  


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update")]
        [TestMethod]
        public async Task UpdateTest()
        {            
            Organization organization = new Organization();
           organization.OrganizationId = "Test";
            organization.Type = "";
            organization.Name = "TestOrg";
            organization.AddressType = "1";
            organization.AddressStreet = null;
            organization.AddressStreetNumber = null;
            organization.PostalCode = null;
            organization.City = null;
            organization.CountryCode = null;
            Organization actual =new Organization();
            _iOrganizationRepository.Setup(s=>s.Update(It.IsAny<Organization>())).ReturnsAsync(actual);
            var result = await _OrganizationManager.Update(organization);
            Assert.AreEqual(result, actual);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete")]
        [TestMethod]
        public async Task DeleteTest()
        {    
            int organizationId= 12;        
           
            _iOrganizationRepository.Setup(s=>s.Delete(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _OrganizationManager.Delete(organizationId);
            Assert.AreEqual(result, true);
        }  


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get")]
        [TestMethod]
        public async Task GetTest()
        {            
             int organizationId= 12;  
            OrganizationResponse actual =new OrganizationResponse();
            _iOrganizationRepository.Setup(s=>s.Get(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _OrganizationManager.Get(organizationId);
            Assert.AreEqual(result, actual);
        }  


        [TestCategory("Unit-Test-Case")]
        [Description("Test for KeyHandOverEvent")]
        [TestMethod]
         public async Task KeyHandOverEventTest()
        {            
            HandOver keyHandOver = new HandOver();

            HandOver actual =new HandOver();
            _iOrganizationRepository.Setup(s=>s.KeyHandOverEvent(It.IsAny<HandOver>())).ReturnsAsync(actual);
            var result = await _OrganizationManager.KeyHandOverEvent(keyHandOver);
            Assert.AreEqual(result, actual);
        }  
    }
}
