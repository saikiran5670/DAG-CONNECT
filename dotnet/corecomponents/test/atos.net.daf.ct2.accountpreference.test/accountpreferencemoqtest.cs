using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.accountpreference;

namespace atos.net.daf.ct2.accountpreference.test
{
[TestClass]
    public class accountpreferencemoqtest
    {
           Mock<IAccountPreferenceRepository> _iAccountPreferenceRepository;
           Mock<IAuditTraillib> __auditlog;
       PreferenceManager _preferenceManager;
        public accountpreferencemoqtest()
        {
            _iAccountPreferenceRepository = new Mock<IAccountPreferenceRepository>();
            __auditlog = new Mock<IAuditTraillib>();
            _preferenceManager = new PreferenceManager(_iAccountPreferenceRepository.Object,__auditlog.Object);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create")]
        [TestMethod]
        public async Task CreateTest()
        {            
            AccountPreference preference = new AccountPreference();
           
            AccountPreference actual =new AccountPreference();
            _iAccountPreferenceRepository.Setup(s=>s.Create(It.IsAny<AccountPreference>())).ReturnsAsync(actual);
            var result = await _preferenceManager.Create(preference);
            Assert.AreEqual(result, actual);
        }   

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update")]
        [TestMethod]
        public async Task UpdateTest()
        {            
            AccountPreference preference = new AccountPreference();
           
            AccountPreference actual =new AccountPreference();
            _iAccountPreferenceRepository.Setup(s=>s.Update(It.IsAny<AccountPreference>())).ReturnsAsync(actual);
            var result = await _preferenceManager.Update(preference);
            Assert.AreEqual(result, actual);
        }     

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get")]
        [TestMethod]
        public async Task GetTest()
        {            
            AccountPreferenceFilter filter = new AccountPreferenceFilter();
           
            var actual =new List<AccountPreference>();
            _iAccountPreferenceRepository.Setup(s=>s.Get(It.IsAny<AccountPreferenceFilter>())).ReturnsAsync(actual);
            var result = await _preferenceManager.Get(filter);
            Assert.AreEqual(result, actual);
        }       
    
    }
}
