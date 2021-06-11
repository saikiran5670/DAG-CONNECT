using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using Identity = net.atos.daf.ct2.identity;

namespace net.atos.daf.ct2.account.test
{
    [TestClass]
    public class AccountManagerTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IAccountRepository _repo;
        private readonly IAccountManager _manager;
        private readonly IAuditTraillib _auditlog;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly Identity.IAccountManager _identity;


        public AccountManagerTest()
        {
            _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
           .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _auditLogRepository = new AuditLogRepository(_dataAccess);
            _auditlog = new AuditTraillib(_auditLogRepository);
            _repo = new AccountRepository(_dataAccess);

            // identity related things 

            var idenityconfiguration = new Identity.IdentityJsonConfiguration()
            {
                Realm = "DAFConnect",
                BaseUrl = "http://104.45.77.70:8080",
                AuthUrl = "/auth/realms/{{realm}}/protocol/openid-connect/token",
                UserMgmUrl = "/auth/admin/realms/{{realm}}/users",
                AuthClientId = "admin-cli",
                AuthClientSecret = "57149493-4055-45cc-abee-fd9f621fc34c",
                UserMgmClientId = "DAF-Admin",
                UserMgmClientSecret = "57149493-4055-45cc-abee-fd9f621fc34c",
                // ReferralUrl="https://dafexternal",
                Issuer = "Me",
                Audience = "You",
                // ReferralId="8c51b38a-f773-4810-8ac5-63b5fb9ca217",
                RsaPrivateKey = "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",
                RsaPublicKey = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAqyFYwF13lXMGZV7/nDiaQ4oPDAH8y23yV0EfSa8Oc0eqnIZd/6GrvirhejmDl5tAJHZANfLbS5Pmj4nScu3SizhoEbb4yhXgp7uJpRGADRAFs9E1v08VBHFQSCaSo4vOXxgrG5UtQjNpSjJWqBIG2kvA6kz1ZDbtK5xaZS+K2vQ64/9o9gYd3Rof/0BqrfMcg0+vq7N7+gTwiDMqcu93EiLbDbIbEQpLohJdQ7DgnxvlcGoPY47mHucR9RALlq0C31U2NDwqErNJZ6BeiSCnRW+aA0mW5zfvD1TS5S9Fdi3Bhb4lEocP/qcfqZC9YYlFu0vhbAz3JJEHIiuVG0V39Rd+De+bi/3Hwj8617+IeuB/pXSBp2C2eTez+dmDewiqFXg5Pv2k3P4FnQU0cbTCj53zIyfwon3p8UF/7wYS1BPMQe2VqhfdjzgvnhLmSd3PXA4gul6gZdSnnUOE0exZ6af1ldqrxi3X3JVqK3S+/WLEpfpCw+nE3jxq/9h+qydcIWr+p0zYwTeh3xxHyGS9dU1SdjwfL4EkDJxxTjAshXOg+4w+IHHFGDpu+nQbm8vQfZTm+NQZFkCsVnueWPthqj3sCz7DL6oh41XCYBPkoFrFXa+e8O3ByMyMs4Uv/5BtIDjXYDHCxF1kY2nR0ySVLWXRAJHgZlt8+8qMbgWSoRsCAwEAAQ=="
            };
            IOptions<Identity.IdentityJsonConfiguration> setting = Options.Create(idenityconfiguration);
            _identity = new Identity.AccountManager(setting);
            _manager = new AccountManager(_repo, _auditlog, _identity, null, null);
        }
        [TestMethod]
        public void CreateAccount()
        {
            Account entity = new Account();
            DateTime dob = new DateTime(1978, 07, 11);
            entity.EmailId = "viranjay.test1@atos.net";
            entity.Salutation = "Mr";
            entity.FirstName = "Viranjay2";
            entity.LastName = "Singh2";
            // entity.Dob = dob;
            entity.AccountType = AccountType.SystemAccount;
            entity.Organization_Id = 1;
            entity.StartDate = DateTime.Now.Ticks;
            entity.EndDate = DateTime.Now.AddYears(2).Ticks;
            // entity.Active = true;
            var result = _manager.Create(entity).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }

        // [TestMethod]
        // public void UpdateAccount()
        // {
        //     Account entity = new Account();     

        //     DateTime dob = new DateTime(1978,07,11);
        //     entity.Id = 5;
        //     entity.EmailId = "viranjay.singh112@gmail.com";
        //     entity.Salutation = "Mr";
        //     entity.FirstName = "Viranjay";
        //     entity.LastName = "Singh";
        //     entity.Dob = dob;
        //     entity.AccountType = AccountType.SystemAccount;
        //     entity.Organization_Id=1;
        //     entity.StartDate = DateTime.Now;            
        //     entity.EndDate = DateTime.Now.AddYears(2);
        //     entity.Active = true;
        //     var result = repository.Update(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }

        // [TestMethod]
        // public void DeleteAccount()
        // {
        //     Account entity = new Account();     
        //     int accountid=1;
        //     int organizationid=1;             
        //     var result = repository.Delete(accountid,organizationid).Result;
        //     Assert.IsTrue(result != null);
        // }

        // [TestMethod]
        // public void GetAccountByAccountId()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=4;
        //     filter.AccountType = AccountType.None;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null);
        // }
        // [TestMethod]
        // public void GetAccountByOrganization()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=0;
        //     filter.OrganizationId =1;
        //     filter.AccountType = AccountType.None;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null );
        // }
        // [TestMethod]
        // public void GetAccountByType()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=0;
        //     filter.OrganizationId = 0;
        //     filter.AccountType = AccountType.SystemAccount;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null );
        // }

        // [TestMethod]
        // public void GetAccountByAccountAndOrganization()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=2;
        //     filter.OrganizationId=1;
        //     filter.AccountType = AccountType.None;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null);
        // }
    }
}
