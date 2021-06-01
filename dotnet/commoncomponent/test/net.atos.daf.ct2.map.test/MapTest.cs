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
        private readonly IConfiguration _config;
        private readonly IMapManager _mapManager;
        public MapTest()
        {
            var cs = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=vehicledatamart;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataMartDataAccess(cs);
            var mapRepository = new MapRepository(_dataAccess);
            _mapManager = new MapManager(mapRepository);
        }
        [TestMethod]
        public void AddLookupAddress()
        {
            var appId = "LRJH9LmTMNbwteXRz03L";
            var appCode = "o9LPYEnoFvNtmkYUhCb1Tg";
            _mapManager.InitializeMapGeocoder(appId, appCode);
            var address = new List<LookupAddress>() { new LookupAddress() { Latitude = 18.50248, Longitude = 73.85704 } };
            var result = _mapManager.AddLookupAddress(address).Result;
            Assert.IsTrue(result.Any(x => x.Id <= 0));
        }
        [TestMethod]
        public void GetLookupAddress()
        {
            var appId = "LRJH9LmTMNbwteXRz03L";
            var appCode = "o9LPYEnoFvNtmkYUhCb1Tg";
            _mapManager.InitializeMapGeocoder(appId, appCode);
            var address = new List<LookupAddress>() { new LookupAddress() { Latitude = 18.50248, Longitude = 73.85704 } };
            var result = _mapManager.GetLookupAddress(address).Result;
            Assert.IsTrue(result.Any(x => x.Id <= 0));
        }




    }
}
