using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.organization.test
{
    [TestClass]
    public class OrganizationManagerTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationManager _organizationManager;
        private readonly IAuditTraillib _auditlog;
        private readonly IVehicleManager _vehicleManager;
        private readonly IGroupManager _groupManager;
        private readonly IAccountManager _accountManager;
        private readonly IAuditLogRepository _auditLogRepository;
        public OrganizationManagerTest()
        {
            _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
           .Build();
            //Get connection string
            // var connectionString = _config.GetConnectionString("Dev");
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _auditLogRepository = new AuditLogRepository(_dataAccess);
            _auditlog = new AuditTraillib(_auditLogRepository);
            _vehicleManager = new VehicleManager(new VehicleRepository(_dataAccess, null));
            _organizationRepository = new OrganizationRepository(_dataAccess, null,
                                                                  _vehicleManager,
                                                                  _groupManager,
                                                                  _accountManager, null, null, null);
            _organizationManager = new OrganizationManager(_organizationRepository, _auditlog);
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
            organization.City = null;
            organization.CountryCode = null;
            // organization.ReferencedDate = 1610372484;
            // organization.OptOutStatus = true;
            organization.OptOutStatusChangedDate = 1610372484;
            organization.IsActive = true;
            var result = _organizationManager.Create(organization).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }

        [TestMethod]
        public void UpdateOrganization_Manager()
        {
            Organization organization = new Organization();
            organization.Id = 1;
            organization.OrganizationId = "Test";
            organization.Type = "";
            organization.Name = "TestOrg-Updated-Manager";
            organization.AddressType = "1";
            organization.AddressStreet = null;
            organization.AddressStreetNumber = null;
            organization.PostalCode = null;
            organization.City = null;
            organization.CountryCode = null;
            // organization.ReferencedDate = 1610372484;
            //  organization.OptOutStatus = true;
            organization.OptOutStatusChangedDate = 1610372484;
            organization.IsActive = true;
            var result = _organizationManager.Update(organization).Result;
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void DeleteOrganization_Manager()
        {
            var result = _organizationManager.Delete(1).Result;
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void GetOrganization_Manager()
        {
            var result = _organizationManager.Get(1).Result;
            Assert.IsTrue(result != null);
        }
    }
}
