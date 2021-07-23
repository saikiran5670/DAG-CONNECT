using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.webservice.entity;

namespace net.atos.daf.ct2.webservice.test
{
    [TestClass]
    public class WebServiceTest
    {

        
        [TestMethod]
        public void GetHttpClientCallTest()
        {
            WebServiceManager webService = new WebServiceManager();
            HeaderDetails headerDetails = new HeaderDetails();
            headerDetails.BaseUrl = "https://api.dev1.ct2.atos.net/login";
            headerDetails.Body = "";
            headerDetails.AuthType = "A";
            headerDetails.UserName = "patilnamita0502@gmail.com";
            headerDetails.Password = "Namita@Patil02";
            headerDetails.ContentType = "application/json";
            var response = webService.HttpClientCall(headerDetails);
            Assert.IsNotNull(response);
        }
    }

}
