using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.poigeofence.entity;
using System;

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
                Name = "Poi Test",
                State = "M",
                // ModifiedAt =,
                //  ModifiedBy =,
                OrganizationId = 100,
                SubCategoryId = 8,
                TripId = 10,
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

            var ObjPoi= new POI()
            {
                Id=26,
                Address = "Atos Syntel Pune",
                CategoryId = 1,
                City = "Pune",
                Country = "India",
                Distance = 22,               
                Name = "Update Poi Test",
                State = "M",               
                ModifiedBy =1,
                OrganizationId = 1,
                SubCategoryId = 2,
                TripId = 2,
                Type = "V",
                Zipcode = "411057",
               

            };
            var resultPackage = _iPoiManager.UpdatePOI(ObjPoi).Result;
            Assert.IsNotNull(resultPackage);

        }



        [TestMethod]
        public void GetPoiTest()
        {
           var poiFilter = new POI() { State = "Maharastra" };
            var result = _iPoiManager.GetAllPOI(poiFilter).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result != null);
        }    
        

        [TestMethod]
        public void DeletePoi()
        {
            var result = _iPoiManager.DeletePOI(2).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result);
        }

    }
}
