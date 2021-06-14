using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.subscription.test
{
    [TestClass]
    public class SubscriptionManagerMoqTest
    {
        Mock<ISubscriptionRepository> _iSubscriptionRepository;
        SubscriptionManager _subscriptionManager;
        public SubscriptionManagerMoqTest()
        {
            _iSubscriptionRepository = new Mock<ISubscriptionRepository>();
            _subscriptionManager = new SubscriptionManager(_iSubscriptionRepository.Object);
        }        
    
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Subscribe")]
        [TestMethod]
        public async Task SubscribeTest()
        {            
            SubscriptionActivation objSubscription = new SubscriptionActivation();
            objSubscription.OrganizationId = "agdydh";
            objSubscription.PackageId = "PKG007";//for type O
            objSubscription.PackageId = "string12";//for type V
            objSubscription.VINs = new List<string>();
            objSubscription.VINs.Add("Vehicle_143_1");
            objSubscription.VINs.Add("Vehicle_143_2");
            objSubscription.VINs.Add("Vehicle_143_3");
            objSubscription.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            Tuple<HttpStatusCode, SubscriptionResponse> actual =new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, new SubscriptionResponse());
            _iSubscriptionRepository.Setup(s=>s.Subscribe(It.IsAny<SubscriptionActivation>())).ReturnsAsync(actual);
            var result = await _subscriptionManager.Subscribe(objSubscription);
            Assert.AreEqual(result.Item1, actual.Item1);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for UnSubscribe")]
         [TestMethod]
          public async Task UnSubscribeTest()
        {
            UnSubscription objUnSubscription = new UnSubscription();
            objUnSubscription.OrganizationID = "agdydh";
            // objUnSubscription.PackageId = "PKG007";//for type O
            // objUnSubscription.PackageId = "string12";//for type V
            objUnSubscription.VINs = new List<string>();
            objUnSubscription.VINs.Add("Vehicle_143_1");
             objUnSubscription.VINs.Add("Vehicle_143_2");
            objUnSubscription.VINs.Add("Vehicle_143_3");
            objUnSubscription.EndDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            Tuple<HttpStatusCode, SubscriptionResponse> actual =new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, new SubscriptionResponse());
             _iSubscriptionRepository.Setup(s => s.Unsubscribe(It.IsAny<UnSubscription>())).ReturnsAsync(actual);
            var result = await _subscriptionManager.Unsubscribe(objUnSubscription);
            
            Assert.AreEqual(result.Item1, actual.Item1);
         }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get")]
        [TestMethod]
        public async Task GetTest()
        {            
            SubscriptionDetailsRequest objSubscriptionDetailsRequest = new SubscriptionDetailsRequest();
            objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            List<SubscriptionDetails> actual =new List<SubscriptionDetails>();
            _iSubscriptionRepository.Setup(s=>s.Get(It.IsAny<SubscriptionDetailsRequest>())).ReturnsAsync(actual);
            var result = await _subscriptionManager.Get(objSubscriptionDetailsRequest);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create")]
        [TestMethod]
        public async Task CreateTest()
        {     

            int orgId = 32;
            int packageId = 34;      
           // SubscriptionDetailsRequest objSubscriptionDetailsRequest = new SubscriptionDetailsRequest();
            //objSubscriptionDetailsRequest.OrganizationId = 23;
           // objSubscriptionDetailsRequest.PackageId = "PKG007";//for type O
           // objSubscriptionDetailsRequest.PackageId = "string12";//for type V
           // objSubscriptionDetailsRequest.VINs = new List<string>();
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_1");
           // objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_2");
            //objSubscriptionDetailsRequest.VINs.Add("Vehicle_143_3");
            //objSubscriptionDetailsRequest.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            SubscriptionResponse actual =new SubscriptionResponse();
            _iSubscriptionRepository.Setup(s=>s.Create(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _subscriptionManager.Create(orgId,packageId);
            Assert.AreEqual(result, actual);
        }
       
        
       
    }
}
