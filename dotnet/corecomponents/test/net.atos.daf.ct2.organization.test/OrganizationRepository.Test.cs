using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration; 
using  net.atos.daf.ct2.audit;
using  net.atos.daf.ct2.vehicle;
using System.Collections.Generic;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;

namespace net.atos.daf.ct2.organization.test
{
     [TestClass]
    public class OrganizationRepositoryTest
    {
         private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IOrganizationRepository _organizationRepository;        
        private readonly IAuditTraillib _auditlog;
        private readonly IVehicleManager _vehicelManager;
        public OrganizationRepositoryTest()
        {
            _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.Test.json")
            .Build();
            //Get connection string
           // var connectionString = _config.GetConnectionString("DevAzure");
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _organizationRepository = new OrganizationRepository(_dataAccess,_vehicelManager); 
        }

        [TestMethod]
        public void CreateOrganization()
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
            var result = _organizationRepository.Create(organization).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }

        [TestMethod]
        public void UpdateOrganization()
        {
            Organization organization = new Organization();  
            organization.Id = 1;          
            organization.OrganizationId = "Test";
            organization.Type = "";
            organization.Name = "TestOrg-Updated";
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
            var result = _organizationRepository.Update(organization).Result;
            Assert.IsTrue(result != null);
        }


        [TestMethod]
        public void DeleteOrganization()
        {
             var result = _organizationRepository.Delete(1).Result;
            Assert.IsTrue(result == true);
        }
        
        [TestMethod]
        public void GetOrganization()
        {          
            var result = _organizationRepository.Get(1).Result;
            Assert.IsTrue(result != null);
        }

         [TestMethod]
        public void KeyHandOverEvent(KeyHandOver keyHandOver)
        {    
            keyHandOver.KeyHandOverEvent.EndCustomer.ID="1";
            keyHandOver.KeyHandOverEvent.VIN="V22";
            keyHandOver.KeyHandOverEvent.TCUActivation="true";
            keyHandOver.KeyHandOverEvent.ReferenceDateTime="04-04-2019";           
            var result = _organizationRepository.KeyHandOverEvent(keyHandOver).Result;
            Assert.IsTrue(result != null);
        }
    }
}
