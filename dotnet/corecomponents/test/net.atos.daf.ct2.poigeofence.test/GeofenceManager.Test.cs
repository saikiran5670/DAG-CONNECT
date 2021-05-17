using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class GeofenceManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly GeofenceRepository _geofenceRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGeofenceManager _iGeofenceManager;

        public GeofenceManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                           .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _categoryRepository = new CategoryRepository(_dataAccess);
            _geofenceRepository = new GeofenceRepository(_dataAccess, _categoryRepository);
            _iGeofenceManager = new GeofenceManager(_geofenceRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for create polygon geofence")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void CreatePolygonGeofence()
        {
            Geofence geofence = new Geofence();
            geofence.OrganizationId = 171;
            geofence.CategoryId = 10;
            geofence.SubCategoryId = 8;
            geofence.Name = "Geofence Unit Test";
            geofence.Type = "P";
            geofence.Address = "Geofence Add Test";
            geofence.City = "Pune";
            geofence.Country = "In";
            geofence.Zipcode = "411018";
            geofence.Latitude = 101.11;
            geofence.Longitude = 100.100;
            geofence.Distance = 0;
          //  geofence.TripId = 0;
            geofence.CreatedBy = 50;
            geofence.Nodes = new List<Nodes>();
            geofence.Nodes.Add(new Nodes() { Latitude = 101.11,Longitude=100.100,SeqNo=1 });
            geofence.Nodes.Add(new Nodes() { Latitude = 102.11, Longitude = 101.101, SeqNo = 2 });
            var resultGeofence = _iGeofenceManager.CreatePolygonGeofence(geofence).Result;
            Assert.IsNotNull(resultGeofence);
            Assert.IsTrue(resultGeofence.Id > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update polygon geofence")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void UpdatePolygonGeofence()
        {
            Geofence geofence = new Geofence();
            geofence.Id = 163;
            geofence.OrganizationId = 171;
            geofence.CategoryId = 10;
            geofence.SubCategoryId = 8;
            geofence.Name = "Geofence Unit Test code";
            geofence.ModifiedBy = 50;
            var resultUpdateGeofence = _iGeofenceManager.UpdatePolygonGeofence(geofence).Result;
            Assert.IsNotNull(resultUpdateGeofence);
            Assert.IsTrue(resultUpdateGeofence.Id > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update circular geofence")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void UpdateCircularGeofence()
        {
            Geofence geofence = new Geofence();
            geofence.Id = 165;
            geofence.OrganizationId = 171;
            geofence.CategoryId = 10;
            geofence.SubCategoryId = 8;
            geofence.Name = "Geofence circular Unit Test code";
            geofence.ModifiedBy = 50;
            var resultUpdateGeofence = _iGeofenceManager.UpdateCircularGeofence(geofence).Result;
            Assert.IsNotNull(resultUpdateGeofence);
            Assert.IsTrue(resultUpdateGeofence.Id > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for create circular geofence")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void CreateCircularGeofence()
        {
            List<Geofence> geofencesList = new List<Geofence>();
            Geofence geofence = new Geofence();
            geofence.OrganizationId = 171;
            geofence.CategoryId = 10;
            geofence.SubCategoryId = 8;
            geofence.Name = "Geofence Unit Test Item 1";
            geofence.Type = "P";
            geofence.Address = "Geofence Add Test";
            geofence.City = "Pune";
            geofence.Country = "In";
            geofence.Zipcode = "411018";
            geofence.Latitude = 101.11;
            geofence.Longitude = 100.100;
            geofence.Distance = 500;
            geofence.CreatedBy = 50;
            geofencesList.Add(geofence);

            Geofence geofence1 = new Geofence();
            geofence1.OrganizationId = 171;
            geofence1.CategoryId = 10;
            geofence1.SubCategoryId = 8;
            geofence1.Name = "Geofence Unit Test Item 2";
            geofence1.Type = "P";
            geofence1.Address = "Geofence Add Test Item 2";
            geofence1.City = "mumbai";
            geofence1.Country = "In";
            geofence1.Zipcode = "400018";
            geofence1.Latitude = 201.11;
            geofence1.Longitude = 200.100;
            geofence1.Distance = 200;
            geofence1.CreatedBy = 50;
            geofencesList.Add(geofence1);
            var resultUpdateGeofence = _iGeofenceManager.CreateCircularGeofence(geofencesList).Result;
            Assert.IsNotNull(resultUpdateGeofence);
            Assert.IsTrue(resultUpdateGeofence.Count > 0);

        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for get all geofence")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetAllGeofence()
        {
            Geofence geofence = new Geofence();
            geofence.OrganizationId = 171;
            //geofence.CategoryId = 10;
            //geofence.SubCategoryId = 8;
            //geofence.Name = "Geofence Unit Test";
            //geofence.Type = "P";
            //geofence.Address = "Geofence Add Test";
            //geofence.City = "Pune";
            //geofence.Country = "In";
            //geofence.Zipcode = "411018";
            //geofence.Latitude = 101.11;
            //geofence.Longitude = 100.100;
            //geofence.Distance = 0;
            //geofence.TripId = 0;
            //geofence.CreatedBy = 50;
            //geofence.Nodes = new List<Nodes>();
            //geofence.Nodes.Add(new Nodes() { Latitude = 101.11, Longitude = 100.100, SeqNo = 1 });
            //geofence.Nodes.Add(new Nodes() { Latitude = 102.11, Longitude = 101.101, SeqNo = 2 });
            var resultGeofence = _iGeofenceManager.GetAllGeofence(geofence).Result;
            Assert.IsNotNull(resultGeofence);
            //Assert.IsTrue(resultGeofence);
        }
    }
}
