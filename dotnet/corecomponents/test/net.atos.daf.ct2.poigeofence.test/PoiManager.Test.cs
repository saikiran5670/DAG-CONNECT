using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class PoiManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private readonly PoiRepository _poiRepository;
        private readonly IPoiManager _iPoiManager;

        public PoiManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                .Build();


            string datamartconnectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=vehicledatamart;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";
            _dataMartDataAccess = new PgSQLDataMartDataAccess(datamartconnectionString);

            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
             _poiRepository = new PoiRepository(_dataAccess,_dataMartDataAccess);
            _iPoiManager = new PoiManager(_poiRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get GlobalPOI details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetAllGobalPOI()
        {
            POIEntityRequest objPOIEntityRequest = new POIEntityRequest();
            objPOIEntityRequest.CategoryId = 10;
            objPOIEntityRequest.SubCategoryId = 8;

            var resultGlobalPOIList = _iPoiManager.GetAllGobalPOI(objPOIEntityRequest).Result;
            Assert.IsNotNull(resultGlobalPOIList);
            Assert.IsTrue(resultGlobalPOIList.Count > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create poi")]
        [TestMethod]
        public void CreatePoiTest()
        {
            var ObjPoi = new POI()
            {
                Address = "Pune",
                CategoryId = 10,
                City = "Pune",
                Country = "India",
                Distance = 12,
                Latitude = 51.07,
                Longitude = 57.07,
                Name = "Poi Test1",
                State = "A",
                // ModifiedAt =,
                //  ModifiedBy =,
                OrganizationId = 100,
                SubCategoryId = 8,
                //TripId = 10,
                Type = "V",
                Zipcode = "411057",
                CreatedBy = 1

            };
            var resultPoi = _iPoiManager.CreatePOI(ObjPoi).Result;
            Assert.IsNotNull(resultPoi);
            Assert.IsTrue(resultPoi.Id > 0);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update package with feature set")]
        [TestMethod]
        public void UpdatePoiTest()
        {

            var ObjPoi = new POI()
            {
                Id = 26,
                Address = "Pune",
                CategoryId = 10,
                City = "Pune",
                Country = "India",
                Distance = 12.00,
                Latitude = 51.07,
                Longitude = 57.07,
                Name = "Poi Update Test12",
                State = "Active",
                // ModifiedAt =,
                ModifiedBy = 1,
                OrganizationId = 100,
                SubCategoryId = 8,
                //TripId = 10,
                Type = "POI",
                Zipcode = "411057",

            };
            var resultPackage = _iPoiManager.UpdatePOI(ObjPoi).Result;
            Assert.IsNotNull(resultPackage);

        }



        [TestMethod]
        public void GetPoiTest()
        {
            var poiFilter = new POI() { };
            var result = _iPoiManager.GetAllPOI(poiFilter).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result != null);
        }


        [TestMethod]
        public void DeletePoi()
        {
            var result = _iPoiManager.DeletePOI(6).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeletePois()
        {
            List<int> ids = new List<int>();
            ids.Add(6);
            ids.Add(13);
            ids.Add(20);
            ids.Add(21);
            ids.Add(22);
            ids.Add(26);
            var result = _iPoiManager.DeletePOI(ids).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateTripArddress( )
        {

            var tripAddressDetails = new TripAddressDetails() {Id= 215015,
                                                               StartAddress= "87160 Saint-Sulpice-les-Feuilles, France",
                                                               EndAddress= "Impasse de la Poste, 41700 Le Controis-en-Sologne, France" };
            var result = _iPoiManager.UpdateTripArddress(tripAddressDetails).Result;
            Console.WriteLine(result);
        }


    }
}
