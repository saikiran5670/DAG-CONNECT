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
    [TestClass]
    public class AccountAssertionManagerTest
    {    
        private readonly IAccountAssertionManager _accountAssertionManager;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly AccountAssertionRepository _accountAssertionRepository;
        public AccountAssertionManagerTest()
        {           
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _accountAssertionRepository = new AccountAssertionRepository(_dataAccess);   
            _accountAssertionManager=new AccountAssertionManager(_accountAssertionRepository) ;
        }

        
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert account assertion")]
        [TestMethod]
        public async Task UnT_identitysession_AccountAssertionManager_InsertAssertion()
        { 
            AccountAssertion accountAssertion =new AccountAssertion();
            accountAssertion.Key="asdfgh";
            accountAssertion.Value="test1";
            accountAssertion.SessionState="2424werwerwersaw342";
            accountAssertion.AccountId="12";
            accountAssertion.CreatedAt="sdfdggrwewew332";

            int result= await _accountAssertionManager.InsertAssertion(accountAssertion);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update account assertion")]
        [TestMethod]
        public async Task UnT_identitysession_AccountAssertionManager_UpdateAssertion()
        { 
            AccountAssertion accountAssertion =new AccountAssertion();
            accountAssertion.Key="asdfgh22";
            accountAssertion.Value="test2";
            accountAssertion.SessionState="2424werwerwersaw34df";
            accountAssertion.AccountId="12";
            accountAssertion.CreatedAt="sdfdggrwewew33ff";

            int result= await _accountAssertionManager.UpdateAssertion(accountAssertion);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete account assertion")]
        [TestMethod]
        public async Task UnT_identitysession_AccountAssertionManager_DeleteAssertion()
        {                      
            int AccountId=123;           
            int result= await _accountAssertionManager.DeleteAssertion(AccountId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account assertion")]
        [TestMethod]
        public async Task UnT_identitysession_AccountAssertionManager_GetAssertion()
        { 
            AccountAssertion accountAssertion =new AccountAssertion();            
            int AccountId=12;           
            var result= await _accountAssertionManager.GetAssertion(AccountId);            
            Assert.IsNotNull(result);
            Assert.IsTrue(result!=null);       
        }

    }
}
