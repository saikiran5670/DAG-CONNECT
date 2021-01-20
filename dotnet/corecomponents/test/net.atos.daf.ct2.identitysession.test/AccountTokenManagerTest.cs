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
    public class AccountTokenManagerTest 
    {
        private readonly IAccountTokenManager _accountTokenManager;
       
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly AccountTokenRepository _accountTokenRepository;
        public AccountTokenManagerTest()
        {   
            string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAF; User Id = postgres; Password = Abcd@1234; CommandTimeout = 90; ";        
            //string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _accountTokenRepository = new AccountTokenRepository(_dataAccess);   
            _accountTokenManager=new AccountTokenManager(_accountTokenRepository) ;
        }
        
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert account Token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_InsertToken()
        { 
            AccountToken accountToken = new AccountToken();     
            accountToken.User_Name = "Test1";
            accountToken.AccessToken = "abcd";
            accountToken.ExpiresIn = 12;
            accountToken.Refresh_Token = "asdf";
            accountToken.Refresh_Expire_In = 1;
            accountToken.Account_Id = 12;                                    
            accountToken.TokenType = "asdr";
            accountToken.Session_Id = 12;
            accountToken.Idp_Type = "asdf";
            accountToken.SessionState = "asdsf2332";
            accountToken.Created_At = 23;
            accountToken.Scope = "asd";
            accountToken.Error = "test";

            int result= await _accountTokenManager.InsertToken(accountToken);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete account Token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_DeleteToken()
        { 
            AccountToken accountToken = new AccountToken();     
            accountToken.User_Name = "Test1";
            accountToken.AccessToken = "abcd";
            accountToken.ExpiresIn = 12;
            accountToken.Refresh_Token = "asdf";
            accountToken.Refresh_Expire_In = 1;
            accountToken.Account_Id = 12;                                    
            accountToken.TokenType = "asdr";
            accountToken.Session_Id = 12;
            accountToken.Idp_Type = "asdf";
            accountToken.SessionState = "asdsf2332";
            accountToken.Created_At = 23;
            accountToken.Scope = "asd";
            accountToken.Error = "test";

            int result= await _accountTokenManager.DeleteToken(accountToken);
            Assert.IsNotNull(result);
            Assert.IsTrue(result>0);           
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_GetTokenDetails()
        {                      
            int AccountId=12;           
            var result= await _accountTokenManager.GetTokenDetails(AccountId);            
            Assert.IsNotNull(result);
            Assert.IsTrue(result!=null);       
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get account token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_GetTokenDetailsbyAccessToken()
        {                      
            string AccessToken="test1";           
            var result= await _accountTokenManager.GetTokenDetails(AccessToken);            
            Assert.IsNotNull(result);
            Assert.IsTrue(result!=null);       
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Validate account token")]
        [TestMethod]
        public async Task UnT_identitysession_AccountTokenManager_ValidateToken()
        {                      
            AccountToken accountToken = new AccountToken();     
            accountToken.User_Name = "Test1";
            accountToken.AccessToken = "abcd";
            accountToken.ExpiresIn = 12;
            accountToken.Refresh_Token = "asdf";
            accountToken.Refresh_Expire_In = 1;
            accountToken.Account_Id = 12;                                    
            accountToken.TokenType = "asdr";
            accountToken.Session_Id = 12;
            accountToken.Idp_Type = "asdf";
            accountToken.SessionState = "asdsf2332";
            accountToken.Created_At = 23;
            accountToken.Scope = "asd";
            accountToken.Error = "test";         
            bool result= await _accountTokenManager.ValidateToken(accountToken);  
            Assert.IsTrue(result);   
        }
    }
}
