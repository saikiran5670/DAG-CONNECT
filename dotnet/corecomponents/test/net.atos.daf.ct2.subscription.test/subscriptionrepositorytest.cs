using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.subscription.test
{
    [TestClass]
    public class Subscriptionrepositorytest
    {
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly SubscriptionRepository _subscriptionRepository;
        public Subscriptionrepositorytest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _subscriptionRepository = new SubscriptionRepository(_dataAccess);
            _subscriptionManager = new SubscriptionManager(_subscriptionRepository);
        }
        /// <summary>
        /// Case if package type is O and has Vins -- fail
        /// Case if package type is V and does'nt have Vins -- fail
        /// if passed package or organization does'nt exists -- fail
        /// </summary>
        /// <returns></returns>
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Subscribe SubscriptionSet ")]
        [TestMethod]
        public async Task UnT_subscribe_SubscriptionManager_SubscribeSubscriptionSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            SubscriptionActivation objSubscription = new SubscriptionActivation();
            objSubscription.OrganizationId = "manoj";
            //objSubscription.packageId = "PKG007";//for type O
            objSubscription.PackageId = "string12";//for type V
            objSubscription.VINs = new List<string>();
            objSubscription.VINs.Add("Vehicle_143_1");
            objSubscription.VINs.Add("Vehicle_143_2");
            objSubscription.VINs.Add("Vehicle_143_3");
            objSubscription.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            var results = await _subscriptionManager.Subscribe(objSubscription);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UnSubscribe SubscriptionSet ")]
        [TestMethod]
        public async Task UnT_subscribe_SubscriptionManager_UnSubscribeSubscriptionSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            UnSubscription objUnSubscription = new UnSubscription();
            //when subscription id sent as orderid
            //objUnSubscription.OrderID = "00000000-0000-0000-0000-000000000000";
            objUnSubscription.OrganizationID = "ddsss";
            //objUnSubscription.PackageId = "PKG007";//for type O
            //objUnSubscription.PackageId = "string12";//for type V
            objUnSubscription.VINs = new List<string>();
            objUnSubscription.VINs.Add("Vehicle_143_1");
            objUnSubscription.VINs.Add("Vehicle_143_2");
            objUnSubscription.VINs.Add("Vehicle_143_3");
            objUnSubscription.EndDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            var results = await _subscriptionManager.Unsubscribe(objUnSubscription);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreatebyOrgId SubscriptionSet ")]
        [TestMethod]
        public async Task UnT_subscribe_SubscriptionManager_CreatebyOrgIdSubscriptionSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            int orgId = 24; int packageId = 75;
            var results = await _subscriptionManager.Create(orgId, packageId);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get SubscriptionSet ")]
        [TestMethod]
        public async Task UnT_subscribe_SubscriptionManager_GetSubscriptionSet()
        {
            SubscriptionDetailsRequest objSubscriptionDetailsRequest = new SubscriptionDetailsRequest();
            //objSubscriptionDetailsRequest.organization_id = 101;
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            var results = await _subscriptionManager.Get(objSubscriptionDetailsRequest);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }
    }
}
