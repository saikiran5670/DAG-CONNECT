using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.organization.test
{
    [TestClass]
    public class OrganizationRepositoryTest
    {
        private readonly IDataAccess _dataAccess;
        readonly IOrganizationRepository _organizationRepository;
        private readonly IVehicleManager _vehicleManager;
        private readonly IGroupManager _groupManager;
        private readonly IAccountManager _accountManager;
        public OrganizationRepositoryTest()
        {
            // _config = new ConfigurationBuilder()
            //  .AddJsonFile("appsettings.Test.json")
            // .Build();
            //Get connection string
            // var connectionString = _config.GetConnectionString("DevAzure");
            var connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";

            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _organizationRepository = new OrganizationRepository(_dataAccess, null,
                                                                  _vehicleManager,
                                                                  _groupManager,
                                                                  _accountManager, null, null, null);





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
            organization.City = null;
            organization.CountryCode = null;
            // organization.ReferencedDate = 1610372484;
            // organization.OptOutStatus = true;
            organization.OptOutStatusChangedDate = 1610372484;
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
            organization.City = null;
            organization.CountryCode = null;
            // organization.ReferencedDate = 1610372484;
            //  organization.OptOutStatus = true;
            organization.OptOutStatusChangedDate = 1610372484;
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
            keyHandOver.KeyHandOverEvent.VIN = "V22";
            keyHandOver.KeyHandOverEvent.TCUID = "TUID";
            keyHandOver.KeyHandOverEvent.EndCustomer.ID = "testing";
            keyHandOver.KeyHandOverEvent.EndCustomer.Name = "NAPA TRUCKS query test";
            keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type = "Home";
            keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street = "Home";
            keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber = "home";
            keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode = "home";
            keyHandOver.KeyHandOverEvent.EndCustomer.Address.City = "home";
            keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode = "home";
            keyHandOver.KeyHandOverEvent.ReferenceDateTime = "01-01-2019";
            keyHandOver.KeyHandOverEvent.TCUActivation = "true";

            //var result = _organizationRepository.KeyHandOverEvent(keyHandOver).Result;
            //Assert.IsTrue(result != null);
        }
    }
}
