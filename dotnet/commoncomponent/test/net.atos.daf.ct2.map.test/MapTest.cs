using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.map.entity;
using net.atos.daf.ct2.map.repository;

namespace net.atos.daf.ct2.map.test
{
    [TestClass]
    public class MapTest
    {
        private readonly IDataMartDataAccess _dataAccess;
        private readonly IMapManager _mapManager;
        public MapTest()
        {
            string cs = "Server=dafct-lan1-d-euwe-cdp-pgsql-datamart.postgres.database.azure.com;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataMartDataAccess(cs);
            var mapRepository = new MapRepository(_dataAccess);
            _mapManager = new MapManager(mapRepository);
        }
        //[TestMethod]
        //public void AddLookupAddress()
        //{
        //    var appId = "LRJH9LmTMNbwteXRz03L";
        //    var appCode = "o9LPYEnoFvNtmkYUhCb1Tg";
        //    _mapManager.InitializeMapGeocoder(appId, appCode);
        //    var address = new List<LookupAddress>() { new LookupAddress() { Latitude = 18.50248, Longitude = 73.85704 } };
        //    var result = _mapManager.AddLookupAddress(address).Result;
        //    Assert.IsTrue(result.Any(x => x.Id <= 0));
        //}
        //[TestMethod]
        //public void GetLookupAddress()
        //{
        //    var appId = "LRJH9LmTMNbwteXRz03L";
        //    var appCode = "o9LPYEnoFvNtmkYUhCb1Tg";
        //    _mapManager.InitializeMapGeocoder(appId, appCode);
        //    var address = new List<LookupAddress>() { new LookupAddress() { Latitude = 19.50248, Longitude = 73.85704 } };
        //    var result = _mapManager.GetLookupAddress(address).Result;
        //    Assert.IsTrue(result.Any(x => x.Id <= 0));

        //}

        [TestMethod]
        public void GetMapAddress()
        {
            var hereMapConfiguration = new HereMapConfiguration()
            {
                AppId = "LRJH9LmTMNbwteXRz03L",
                AppCode = "o9LPYEnoFvNtmkYUhCb1Tg"
            };
            _mapManager.InitializeMapGeocoder(hereMapConfiguration);
            LookupAddress address = new LookupAddress() { Latitude = 29.50249, Longitude = 94.85703 };
            LookupAddress result = _mapManager.GetMapAddress(address).Result;
            Assert.IsTrue(string.IsNullOrEmpty(result.Address));
        }




    }
}
