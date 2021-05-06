using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class CorridorManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly CorridorRepository _CorridorRepository;
        private readonly ICorridorManger _iCorridorManger;

        public CorridorManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _CorridorRepository = new CorridorRepository(_dataAccess);
            _iCorridorManger = new CorridorManger(_CorridorRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Corridor List By OrgId details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCorridorListByOrgId()
        {
            CorridorRequest objCorridorRequest = new CorridorRequest();
            objCorridorRequest.OrganizationId = 100;//orgid 5 ,100

            var resultCorridorList = _iCorridorManger.GetCorridorList(objCorridorRequest).Result;
            Assert.IsNotNull(resultCorridorList);
            Assert.IsTrue(resultCorridorList.GridView.Count > 0);

        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Corridor List By OrgId and CorriId details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCorridorListByOrgIdandCorriId()
        {
            CorridorRequest objCorridorRequest = new CorridorRequest();
            objCorridorRequest.OrganizationId = 100;//orgid 5 ,100
            objCorridorRequest.CorridorId = 172; //landmark table id 109, 172

            var resultCorridorList = _iCorridorManger.GetCorridorList(objCorridorRequest).Result;
            Assert.IsNotNull(resultCorridorList);
            Assert.IsTrue(resultCorridorList.EditView.Count > 0);

        }
    }
}
