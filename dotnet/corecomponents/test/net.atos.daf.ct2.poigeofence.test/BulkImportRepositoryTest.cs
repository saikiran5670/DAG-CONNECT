using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class BulkImportRepositoryTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly GeofenceRepository _geofenceRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGeofenceManager _geofenceManager;

        public BulkImportRepositoryTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _categoryRepository = new CategoryRepository(_dataAccess);
            _geofenceRepository = new GeofenceRepository(_dataAccess, _categoryRepository);
            _geofenceManager = new GeofenceManager(_geofenceRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Bulk Import Geofence Success")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void BulkImportGeofence_Success()
        {
            //Change the name fields, should not be exist in database for the given org id =1
            var randomNumber = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var geofences = new List<Geofence>
            {
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1 , Type = ((char)LandmarkType.CircularGeofence).ToString(), Distance = 10,Latitude=110,Longitude=10,CreatedBy = 0,City = string.Empty},
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1 , Type = ((char)LandmarkType.CircularGeofence).ToString(), Distance = 10,Latitude=110,Longitude=10,CreatedBy = 0,City = string.Empty},
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1, Type = ((char)LandmarkType.PolygonGeofence).ToString(), Distance = 10, Latitude = 110, Longitude = 10, CreatedBy = 0,City = string.Empty,
                                Nodes = new List<Nodes>
                                {
                                    new Nodes { SeqNo=1, Latitude=110,Longitude=10,CreatedBy=0},
                                    new Nodes {  SeqNo=1, Latitude=120,Longitude=20,CreatedBy=0}
                                }
                            }
            };

            var resultgeofenceIList = _geofenceManager.BulkImportGeofence(geofences).Result;
            Assert.IsNotNull(resultgeofenceIList);
            Assert.IsTrue(resultgeofenceIList.Where(w => w.IsFailed == false).Count() == 3);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Bulk Import Geofence failure")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void BulkImportGeofence_Failure()
        {
            //Change the name fields, same as per above Success method
            var randomNumber = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var geofences = new List<Geofence>
            {
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1 , Type = ((char)LandmarkType.CircularGeofence).ToString(), Distance = 10,Latitude=110,Longitude=10,CreatedBy = 0,City = string.Empty},
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1 , Type = ((char)LandmarkType.CircularGeofence).ToString(), Distance = 10,Latitude=110,Longitude=10,CreatedBy = 0,City = string.Empty},
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1, Type = ((char)LandmarkType.PolygonGeofence).ToString(), Distance = 10, Latitude = 110, Longitude = 10, CreatedBy = 0,City = string.Empty,
                                Nodes = new List<Nodes>
                                {
                                    new Nodes { SeqNo=1, Latitude=110,Longitude=10,CreatedBy=0},
                                    new Nodes {  SeqNo=1, Latitude=120,Longitude=20,CreatedBy=0}
                                }
                            }
            };

            var resultgeofenceIList = _geofenceManager.BulkImportGeofence(geofences).Result;
            var geofencesOld = new List<Geofence>
            {
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1 , Type = ((char)LandmarkType.CircularGeofence).ToString(), Distance = 10,Latitude=110,Longitude=10,CreatedBy = 0,City = string.Empty},
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1 , Type = ((char)LandmarkType.CircularGeofence).ToString(), Distance = 10,Latitude=110,Longitude=10,CreatedBy = 0,City = string.Empty},
                new Geofence { Name = $"tsgeo{randomNumber}", OrganizationId = 1, Type = ((char)LandmarkType.PolygonGeofence).ToString(), Distance = 10, Latitude = 110, Longitude = 10, CreatedBy = 0,City = string.Empty,
                                Nodes = new List<Nodes>
                                {
                                    new Nodes { SeqNo=1, Latitude=110,Longitude=10,CreatedBy=0},
                                    new Nodes {  SeqNo=1, Latitude=120,Longitude=20,CreatedBy=0}
                                }
                            }
            };
            resultgeofenceIList = _geofenceManager.BulkImportGeofence(geofencesOld).Result;

            Assert.IsNotNull(resultgeofenceIList);
            Assert.IsTrue(resultgeofenceIList.Where(w => w.IsFailed == true).Count() == 3);
        }

    }
}
