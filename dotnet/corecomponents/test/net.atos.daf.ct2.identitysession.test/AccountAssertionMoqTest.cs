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
    public class AccountAssertionMoqTest
    {
Mock<IAccountAssertionRepository> _iAccountAssertionRepository;
        AccountAssertionManager _AccountAssertionManager;
        public AccountAssertionMoqTest()
        {
            _iAccountAssertionRepository = new Mock<IAccountAssertionRepository>();
            _AccountAssertionManager = new AccountAssertionManager(_iAccountAssertionRepository.Object);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for InsertAssertion")]
        [TestMethod]
        public async Task InsertAssertionTest()
        {            
            AccountAssertion accountAssertion = new AccountAssertion();
           accountAssertion.Key = "Unit";
            accountAssertion.Value = "Kg";
            accountAssertion.SessionState = "5e9c64fa-8cac-420c-8f7a-d7a12fe4c0b5";
            accountAssertion.AccountId = "4";
            accountAssertion.CreatedAt = "1";
            //List<SubscriptionDetails> actual =new List<SubscriptionDetails>();
            _iAccountAssertionRepository.Setup(s=>s.InsertAssertion(It.IsAny<AccountAssertion>())).ReturnsAsync(12);
            var result = await _AccountAssertionManager.InsertAssertion(accountAssertion);
            Assert.AreEqual(result, 12);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateAssertion")]
        [TestMethod]
        public async Task UpdateAssertionTest()
        {            
            AccountAssertion accountAssertion = new AccountAssertion();
           accountAssertion.Key = "Unit";
            accountAssertion.Value = "Kg";
            accountAssertion.SessionState = "5e9c64fa-8cac-420c-8f7a-d7a12fe4c0b5";
            accountAssertion.AccountId = "4";
            accountAssertion.CreatedAt = "1";
            //List<SubscriptionDetails> actual =new List<SubscriptionDetails>();
            _iAccountAssertionRepository.Setup(s=>s.UpdateAssertion(It.IsAny<AccountAssertion>())).ReturnsAsync(12);
            var result = await _AccountAssertionManager.UpdateAssertion(accountAssertion);
            Assert.AreEqual(result, 12);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteAssertion")]
        [TestMethod]
        public async Task DeleteAssertionTest()
        {            
            int accountId=12;
            //List<SubscriptionDetails> actual =new List<SubscriptionDetails>();
            _iAccountAssertionRepository.Setup(s=>s.DeleteAssertion(It.IsAny<int>())).ReturnsAsync(12);
            var result = await _AccountAssertionManager.DeleteAssertion(accountId);
            Assert.AreEqual(result, 12);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteAssertionbySessionId")]
        [TestMethod]
        public async Task DeleteAssertionbySessionIdTest()
        {            
            int sessionId=12;
            //List<SubscriptionDetails> actual =new List<SubscriptionDetails>();
            _iAccountAssertionRepository.Setup(s=>s.DeleteAssertionbySessionId(It.IsAny<int>())).ReturnsAsync(12);
            var result = await _AccountAssertionManager.DeleteAssertionbySessionId(sessionId);
            Assert.AreEqual(result, 12);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetAssertion")]
        [TestMethod]
        public async Task GetAssertionTest()
        {            
            int accountId=12;
            var actual =new List<AccountAssertion>();
            _iAccountAssertionRepository.Setup(s=>s.GetAssertion(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _AccountAssertionManager.GetAssertion(accountId);
            Assert.AreEqual(result, actual);
        }                  

    }
}
