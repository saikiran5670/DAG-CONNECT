using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.identitysession.test
{
    [TestClass]
    public class AccountSessionManagerTest
    {
        private readonly IAccountSessionManager _accountSessionManager;
        private readonly IDataAccess _dataAccess;
        private readonly AccountSessionRepository _accountSessionRepository;
        public AccountSessionManagerTest()
        {
            //string connectionString = "Server = localhost; Port = 5432; Database = DAF; User Id = postgres; Password = Abcd@1234; CommandTimeout = 90; ";
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _accountSessionRepository = new AccountSessionRepository(_dataAccess);
            _accountSessionManager = new AccountSessionManager(_accountSessionRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_InsertSession()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            AccountSession accountsession = new AccountSession();
            accountsession.Id = 1;
            accountsession.IpAddress = "12.123.46.12";
            accountsession.LastSessionRefresh = 235;
            accountsession.SessionStartedAt = iSessionStartedAt;
            accountsession.SessionExpiredAt = iSessionExpireddAt;
            accountsession.AccountId = 4;
            accountsession.CreatedAt = 1;

            int result = await _accountSessionManager.InsertSession(accountsession);
            Assert.IsTrue(result > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_UpdateSession()
        {
            AccountSession accountsession = new AccountSession();
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(1));
            accountsession.Id = 1;
            accountsession.IpAddress = "12.123.46.12";
            accountsession.LastSessionRefresh = 232;
            accountsession.SessionStartedAt = iSessionStartedAt;
            accountsession.SessionExpiredAt = iSessionExpireddAt;
            accountsession.AccountId = 4;
            accountsession.CreatedAt = 1;

            int result = await _accountSessionManager.UpdateSession(accountsession);
            Assert.IsTrue(result > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_DeleteSession()
        {

            string SessionID = "9d8fce48-b918-4e51-a030-4a781ed7e156";
            int result = await _accountSessionManager.DeleteSession(SessionID);
            Assert.IsTrue(result > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_GetAccountSession()
        {
            int AccountId = 4;
            var result = await _accountSessionManager.GetAccountSession(AccountId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }
    }
}
