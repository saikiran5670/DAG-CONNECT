using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class PoiManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly PoiRepository _poiRepository;
        private readonly IPoiManager _iPoiManager;

        public PoiManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _poiRepository = new PoiRepository(_dataAccess);
            _iPoiManager = new PoiManager(_poiRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get GlobalPOI details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetAllGobalPOI()
        {
            POIEntityRequest objPOIEntityRequest = new POIEntityRequest();
            objPOIEntityRequest.CategoryId = 4;
            //objPOIEntityRequest.SubCategoryId = 5;

             var resultGlobalPOIList = _iPoiManager.GetAllGobalPOI(objPOIEntityRequest).Result;
            Assert.IsNotNull(resultGlobalPOIList);
            Assert.IsTrue(resultGlobalPOIList.Count > 0);

        }

        [TestMethod]
        public void CreatePoi()
        {
        }
    }
}
