using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration; 
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.organization.repository;
using System.Collections.Generic;
using net.atos.daf.ct2.organization.entity;
namespace net.atos.daf.ct2.organization.test
{
    [TestClass]
    public class OrganizationManagerTest
    {  private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IOrganizationRepository _organizationRepository;        
        private readonly IOrganizationManager _organizationManager;
        private readonly IAuditTraillib _auditlog;
         private readonly IAuditLogRepository _auditLogRepository;
        public OrganizationManagerTest()
        {
             _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.Test.json")
            .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("Dev");
            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _auditLogRepository=new AuditLogRepository(_dataAccess);
            _auditlog= new AuditTraillib(_auditLogRepository);
            _organizationRepository = new OrganizationRepository(_dataAccess);
            _organizationManager = new OrganizationManager(_organizationRepository,_auditlog);
        }

        [TestMethod]
        public void CreateOrganization_Manager()
        {
            Organization organization = new Organization();     
            organization.OrganizationId = "Test";
            organization.Type = "";
            organization.Name = "TestOrg";
            organization.AddressType = "1";
            organization.AddressStreet = null;
            organization.AddressStreetNumber = null;
            organization.PostalCode = null;
            organization.City =null;
            organization.CountryCode = null;
            organization.ReferencedDate = 1610372484;
            organization.OptOutStatus = true;
            organization.OptOutStatusChangedDate =1610372484;
            organization.IsActive = true;
            var result = _organizationManager.Create(organization).Result;
            Assert.IsTrue(result != null && result.Id > 0);            
        }

        [TestMethod]
        public void UpdateOrganization_Manager()
        {
            Organization organization = new Organization();            
            organization.OrganizationId = "Test";
            organization.Type = "";
            organization.Name = "TestOrg-Updated-Manager";
            organization.AddressType = "1";
            organization.AddressStreet = null;
            organization.AddressStreetNumber = null;
            organization.PostalCode = null;
            organization.City =null;
            organization.CountryCode = null;
            organization.ReferencedDate = 1610372484;
            organization.OptOutStatus = true;
            organization.OptOutStatusChangedDate =1610372484;
            organization.IsActive = true;
            var result = _organizationManager.Update(organization).Result;
            Assert.IsTrue(result != null);
        }

         [TestMethod]
        public void DeleteOrganization_Manager()
        {
            var result = _organizationManager.Delete("Test").Result;
            Assert.IsTrue(result == true);
        }
        
        [TestMethod]
        public void GetOrganization_Manager()
        {          
            var result = _organizationManager.Get("Test").Result;
            Assert.IsTrue(result != null);
        }
    }
}
