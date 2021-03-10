using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.utilities;
using System;
using net.atos.daf.ct2.subscription.entity;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.subscription.test
{
    [TestClass]
    public class Subscriptionrepositorytest
    {
        private readonly ISubscriptionManager _SubscriptionManager;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly SubscriptionRepository _subscriptionRepository;
        public Subscriptionrepositorytest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
             var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _subscriptionRepository = new SubscriptionRepository(_dataAccess);
            _SubscriptionManager = new SubscriptionManager(_subscriptionRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Subscribe SubscriptionSet ")]
        [TestMethod]
        public async Task UnT_subscribe_SubscriptionManager_SubscribeSubscriptionSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            Subscription objSubscription = new Subscription();
            objSubscription.OrganizationId = $"Subscription {iSessionStartedAt}";
            objSubscription.packageId = "";
            objSubscription.VINs[0] = "v369369";
            objSubscription.VINs[1] = "v369370";
            objSubscription.VINs[2] = "v369371";
            var results = await _SubscriptionManager.Subscribe(objSubscription);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UnSubscribe SubscriptionSet ")]
        [TestMethod]
        public async Task UnT_subscribe_SubscriptionManager_UnSubscribeSubscriptionSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            Subscription objSubscription = new Subscription();
            objSubscription.OrganizationId = $"Subscription {iSessionStartedAt}";
            objSubscription.packageId = "";
            objSubscription.VINs[0] = "v369369";
            objSubscription.VINs[1] = "v369370";
            objSubscription.VINs[2] = "v369371";
            var results = await _SubscriptionManager.Subscribe(objSubscription);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }
    }
}
