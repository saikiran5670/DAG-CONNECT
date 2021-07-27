using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.webservice.entity;

namespace net.atos.daf.ct2.webservice.test
{
    [TestClass]
    public class WebServiceTest
    {

        [TestCategory("Unit-Test-Case")]
        [TestMethod]
        [Description("Web Service Call")]
        [Timeout(TestTimeout.Infinite)]
        public void GetHttpClientCallTest()
        {
            WebServiceManager webService = new WebServiceManager();
            HeaderDetails headerDetails = new HeaderDetails();
            headerDetails.BaseUrl = "https://api.dev1.ct2.atos.net/login"; //"https://localhost:44300/login";//
            headerDetails.Body = "Hello Test";
            headerDetails.AuthType = "A";
            headerDetails.UserName = "patilnamita0502@gmail.com";
            headerDetails.Password = "Namita@Patil02";
            headerDetails.ContentType = "application/json";
            var response = webService.HttpClientCall(headerDetails).Result;
            Assert.IsNotNull(response);
        }
    }

}
