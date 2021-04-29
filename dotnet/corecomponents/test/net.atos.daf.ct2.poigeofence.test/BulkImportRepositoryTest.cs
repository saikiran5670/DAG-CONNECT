using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System;
using System.Collections.Generic;
using System.Linq;

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
            _geofenceRepository = new PoiRepository(_dataAccess);
        }

    }
}
