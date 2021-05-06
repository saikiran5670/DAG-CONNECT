using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.alert.repository;
using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.test
{
    [TestClass]
    public class AlertManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly AlertRepository _alertRepository;
        private readonly IAlertManager _ialertManager;
        public AlertManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _alertRepository = new AlertRepository(_dataAccess);
            _ialertManager = new AlertManager(_alertRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Activate Alert Success")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void ActivateAlertSuccess()
        {
            //Provide the Alert Id which has suspended State in Database
            var ExcepteId = 1;
            var Id =  _ialertManager.ActivateAlert(ExcepteId, ((char)AlertState.Active)).Result;
            Assert.AreEqual(ExcepteId, Id);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Activate Alert Failure")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void ActivateAlertfailure()
        {
            var ExcepteId = 0;
            var Id = _ialertManager.ActivateAlert(ExcepteId, ((char)AlertState.Active)).Result;
            Assert.AreEqual(ExcepteId, Id);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for create Alert")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void CreateAlertTest()
        {
            Alert alert = new Alert();
            alert.Name = "Test";
            var result = _ialertManager.CreateAlert(alert).Result;
            Assert.IsTrue(result.Id > 0);
        }
    }
}
