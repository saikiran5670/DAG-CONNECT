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
    public class AccountSessionMoqTest
    {

        Mock<IAccountSessionRepository> _iAccountSessionRepository;
        AccountSessionManager _IAccountSessionManager;
        public AccountSessionMoqTest()
        {
            _iAccountSessionRepository = new Mock<IAccountSessionRepository>();
            _IAccountSessionManager = new AccountSessionManager(_iAccountSessionRepository.Object);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for InsertSession")]
        [TestMethod]
        public async Task InsertSessionTest()
        {   
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));         
            AccountSession accountSession = new AccountSession();
            accountSession.Id = 1;
            accountSession.IpAddress = "12.123.46.12";
            accountSession.LastSessionRefresh = 235;
            accountSession.SessionStartedAt = iSessionStartedAt;
            accountSession.SessionExpiredAt = iSessionExpireddAt;
            accountSession.AccountId = 4;
            accountSession.CreatedAt = 1;
            _iAccountSessionRepository.Setup(s=>s.InsertSession(It.IsAny<AccountSession>())).ReturnsAsync(1);
            var result = await _IAccountSessionManager.InsertSession(accountSession);
            Assert.AreEqual(result, 1);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateSession")]
        [TestMethod]
        public async Task UpdateSessionTest()
        {   
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));         
            AccountSession accountSession = new AccountSession();
            accountSession.Id = 1;
            accountSession.IpAddress = "12.123.46.12";
            accountSession.LastSessionRefresh = 235;
            accountSession.SessionStartedAt = iSessionStartedAt;
            accountSession.SessionExpiredAt = iSessionExpireddAt;
            accountSession.AccountId = 4;
            accountSession.CreatedAt = 1;
            _iAccountSessionRepository.Setup(s=>s.UpdateSession(It.IsAny<AccountSession>())).ReturnsAsync(1);
            var result = await _IAccountSessionManager.UpdateSession(accountSession);
            Assert.AreEqual(result, 1);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteSession")]
        [TestMethod]
        public async Task DeleteSessionTest()
        {   
           string SessionId="3474448484";
            //accountSession.LastSessionRefresh = 235;
            //accountSession.SessionStartedAt = iSessionStartedAt;
           // accountSession.SessionExpiredAt = iSessionExpireddAt;
           // accountSession.AccountId = 4;
           // accountSession.CreatedAt = 1;
            _iAccountSessionRepository.Setup(s=>s.DeleteSession(It.IsAny<string>())).ReturnsAsync(1);
            var result = await _IAccountSessionManager.DeleteSession(SessionId);
            Assert.AreEqual(result, 1);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAccountSession")]
        [TestMethod]
        public async Task GetAccountSessionTest()
        {   
           int AccountId=23;
        var actual = new List<AccountSession>();
            _iAccountSessionRepository.Setup(s=>s.GetAccountSession(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _IAccountSessionManager.GetAccountSession(AccountId);
            Assert.AreEqual(result, actual);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteSessionByAccountId")]
        [TestMethod]
        public async Task DeleteSessionByAccountIdTest()
        {   
           int SessionId=34;
            
            _iAccountSessionRepository.Setup(s=>s.DeleteSessionByAccountId(It.IsAny<int>())).ReturnsAsync(1);
            var result = await _IAccountSessionManager.DeleteSessionByAccountId(SessionId);
            Assert.AreEqual(result, 1);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAccountSessionById")]
        [TestMethod]
        public async Task GetAccountSessionByIdTest()
        {   
           int AccountId=23;
        AccountSession actual = new AccountSession();
            _iAccountSessionRepository.Setup(s=>s.GetAccountSessionById(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _IAccountSessionManager.GetAccountSessionById(AccountId);
            Assert.AreEqual(result, actual);
        } 

    }
}
