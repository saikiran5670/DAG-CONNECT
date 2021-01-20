using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.identitysession;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.identitysession.repository;

namespace net.atos.daf.ct2.identitysession.test
{
    public class AccountSessionManagerTest 
    {
        private readonly IAccountSessionManager _accountSessionManager;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly AccountSessionRepository _accountSessionRepository;
        public AccountSessionManagerTest()
        { 
            string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAF; User Id = postgres; Password = Abcd@1234; CommandTimeout = 90; ";                  
            //string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _accountSessionRepository = new AccountSessionRepository(_dataAccess);   
            _accountSessionManager=new AccountSessionManager(_accountSessionRepository) ;
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_InsertSession()
        { 
            AccountSession accountsession = new AccountSession();     
            accountsession.IpAddress = "12.123.46.12";
            accountsession.LastSessionRefresh = 235;
            accountsession.SessionStartedAt = 23;
            accountsession.SessionExpiredAt = 234;
            accountsession.AccountId =12;
            accountsession.CreatedAt = 12;

            int result= await _accountSessionManager.InsertSession(accountsession);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_UpdateSession()
        { 
            AccountSession accountsession = new AccountSession();     
            accountsession.IpAddress = "12.123.46.12";
            accountsession.LastSessionRefresh = 232;
            accountsession.SessionStartedAt = 21;
            accountsession.SessionExpiredAt = 21;
            accountsession.AccountId =12;
            accountsession.CreatedAt = 12;

            int result= await _accountSessionManager.UpdateSession(accountsession);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountSessionManager_DeleteSession()
        { 
            AccountSession accountsession = new AccountSession();     
            accountsession.IpAddress = "12.123.46.12";
            accountsession.LastSessionRefresh = 232;
            accountsession.SessionStartedAt = 21;
            accountsession.SessionExpiredAt = 21;
            accountsession.AccountId =12;
            accountsession.CreatedAt = 12;

            int result= await _accountSessionManager.DeleteSession(accountsession);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account session")]
        [TestMethod]
        public async Task UnT_identitysession_AccountAssertionManager_GetAccountSession()
        { 
            AccountAssertion accountAssertion =new AccountAssertion();            
            int AccountId=12;           
            var result= await _accountSessionManager.GetAccountSession(AccountId);            
            Assert.IsNotNull(result);
            Assert.IsTrue(result!=null);       
        }
    }
}
