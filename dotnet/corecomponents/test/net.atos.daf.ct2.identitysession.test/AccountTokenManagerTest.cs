using System;
using System.Collections.Generic;
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
    public class AccountTokenManagerTest
    {
        private readonly IAccountTokenManager _accountTokenManager;

        private readonly IDataAccess _dataAccess;
        private readonly AccountTokenRepository _accountTokenRepository;
        public AccountTokenManagerTest()
        {
            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAF; User Id = postgres; Password = Abcd@1234; CommandTimeout = 90; ";        
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _accountTokenRepository = new AccountTokenRepository(_dataAccess);
            _accountTokenManager = new AccountTokenManager(_accountTokenRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert account Token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_InsertToken()
        {
            AccountToken accountToken = new AccountToken();
            long iExpireIn = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            long iRefreshExpireIn = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            accountToken.UserName = "Test2";
            accountToken.AccessToken = "abcde";
            accountToken.ExpireIn = 123;
            //accountToken.RefreshToken = "asdf";
            //accountToken.RefreshExpireIn = 12345;
            accountToken.AccountId = 4;
            accountToken.TokenType = ENUM.TokenType.Authentication;
            accountToken.SessionState = "5e9c64fa-8cac-420c-8f7a-d7a12fe4c0b5";
            accountToken.IdpType = ENUM.IDPType.Keycloak;
            accountToken.CreatedAt = 4;
            accountToken.Scope = "asd";
            accountToken.Error = "test";

            int result = await _accountTokenManager.InsertToken(accountToken);
            Assert.IsTrue(result > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete account Token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_DeleteToken()
        {
            List<string> tokenID = new List<string>();
            //TODO
            //Data preparing
            tokenID.Add("6392d8af-add3-49ac-9fa6-413e73cbb885");
            tokenID.Add("929d03c6-1111-4e93-bfda-3996f1c9dd4b");
            int result = await _accountTokenManager.DeleteToken(tokenID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete account Token by session id")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_DeleteTokenBySessionId()
        {
            int sessionId = 1;
            int result = await _accountTokenManager.DeleteTokenbySessionId(sessionId);
            Assert.IsTrue(result > 0);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_GetTokenDetails()
        {
            int AccountId = 4;
            var result = await _accountTokenManager.GetTokenDetails(AccountId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_GetTokenDetailsbyAccessToken()
        {
            string Token_Id = "38bf8a64-e270-4e5b-a107-3b8125f34669";
            var result = await _accountTokenManager.GetTokenDetails(Token_Id);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Validate account token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_ValidateToken()
        {
            string TokenId = "38bf8a64-e270-4e5b-a107-3b8125f34669";

            bool result = await _accountTokenManager.ValidateToken(TokenId);
            Assert.IsTrue(result);
        }
    }
}
