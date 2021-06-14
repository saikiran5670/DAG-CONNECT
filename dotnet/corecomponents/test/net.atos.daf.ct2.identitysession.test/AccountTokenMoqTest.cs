using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.identitysession.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.identitysession.test
{
    [TestClass]
    public class AccountTokenMoqTest
    {
        Mock<IAccountTokenRepository> _iAccountTokenRepository;
        AccountTokenManager _AccountTokenManager;
        public AccountTokenMoqTest()
        {
            _iAccountTokenRepository = new Mock<IAccountTokenRepository>();
            _AccountTokenManager = new AccountTokenManager(_iAccountTokenRepository.Object);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for InsertToken")]
        [TestMethod]
        public async Task InsertTokenTest()
        {            
            AccountToken accountToken = new AccountToken();
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
            _iAccountTokenRepository.Setup(s=>s.InsertToken(It.IsAny<AccountToken>())).ReturnsAsync(2);
            var result = await _AccountTokenManager.InsertToken(accountToken);
            Assert.AreEqual(result, 2);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteToken")]
        [TestMethod]
        public async Task DeleteTokenTest()
        {            
            List<string> token_Id = new List<string>();
          
            _iAccountTokenRepository.Setup(s=>s.DeleteToken(It.IsAny<List<string>>())).ReturnsAsync(2);
            var result = await _AccountTokenManager.DeleteToken(token_Id);
            Assert.AreEqual(result, 2);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteTokenbySessionId")]
        [TestMethod]
        public async Task DeleteTokenbySessionIdTest()
        {            
           int sessionID =12;
          
            _iAccountTokenRepository.Setup(s=>s.DeleteTokenbySessionId(It.IsAny<int>())).ReturnsAsync(12);
            var result = await _AccountTokenManager.DeleteTokenbySessionId(sessionID);
            Assert.AreEqual(result, 12);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetTokenDetails")]
        [TestMethod]
        public async Task GetTokenDetailsTest()
        {            
           int AccountID =12;
          var actual = new List<AccountToken>();
            _iAccountTokenRepository.Setup(s=>s.GetTokenDetails(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _AccountTokenManager.GetTokenDetails(AccountID);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetTokenDetails")]
        [TestMethod]
        public async Task GetTokenDetails1Test()
        {            
           string TokenId = "2324345";
          var actual = new List<AccountToken>();
            _iAccountTokenRepository.Setup(s=>s.GetTokenDetails(It.IsAny<string>())).ReturnsAsync(actual);
            var result = await _AccountTokenManager.GetTokenDetails(TokenId);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for ValidateToken")]
        [TestMethod]
        public async Task ValidateTokenTest()
        {            
            string TokenId = "2324345";
          
            _iAccountTokenRepository.Setup(s=>s.ValidateToken(It.IsAny<string>())).ReturnsAsync(true);
            var result = await _AccountTokenManager.ValidateToken(TokenId);
            Assert.AreEqual(result, true);
        }

         [TestCategory("Unit-Test-Case")]
        [Description("Test for GetTokenCount")]
        [TestMethod]
        public async Task GetTokenCountTest()
        {            
           int AccountID =12;
          //var actual = new List<AccountToken>();
            _iAccountTokenRepository.Setup(s=>s.GetTokenCount(It.IsAny<int>())).ReturnsAsync(12);
            var result = await _AccountTokenManager.GetTokenCount(AccountID);
            Assert.AreEqual(result, 12);
        }

         [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteTokenbyAccountId")]
        [TestMethod]
        public async Task DeleteTokenbyAccountIdTest()
        {            
           int sessionID =12;
          //var actual = new List<AccountToken>();
            _iAccountTokenRepository.Setup(s=>s.DeleteTokenbySessionId(It.IsAny<int>())).ReturnsAsync(1);
            var result = await _AccountTokenManager.DeleteTokenbyAccountId(sessionID);
            Assert.AreEqual(result, 1);
        }
  

    }
}
