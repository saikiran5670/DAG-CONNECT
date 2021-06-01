using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.utilities;



namespace net.atos.daf.ct2.feature.test
{
    [TestClass]
    public class featurerepositorytest
    {
        private readonly IFeatureManager _FeatureManager;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly FeatureRepository _featureRepository;
        public featurerepositorytest()
        {
            // string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y97;Ssl Mode=Require;";

            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();

            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _featureRepository = new FeatureRepository(_dataAccess);
            _FeatureManager = new FeatureManager(_featureRepository);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert FeatureSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_CreateFeatureSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            FeatureSet feature = new FeatureSet();
            feature.Name = "FeatureSet_" + iSessionStartedAt;
            feature.description = null;
            feature.State = 'A';
            feature.created_at = iSessionStartedAt;
            feature.created_by = 1;
            feature.modified_at = iSessionExpireddAt;
            feature.modified_by = 1;

            feature.Features = new List<features.entity.Feature>();
            features.entity.Feature objfeature = new features.entity.Feature();
            objfeature.Id = 4;
            features.entity.Feature objfeature1 = new features.entity.Feature();
            objfeature1.Id = 2;
            features.entity.Feature objfeature2 = new features.entity.Feature();
            objfeature2.Id = 3;

            feature.Features.Add(objfeature);
            feature.Features.Add(objfeature1);
            feature.Features.Add(objfeature2);

            var result = await _FeatureManager.CreateFeatureSet(feature);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Insert DataattributeSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_CreateDataattributeSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            DataAttributeSet dataAttributeSet = new DataAttributeSet();
            dataAttributeSet.Name = "AttributeSet_" + iSessionStartedAt;
            dataAttributeSet.State = 'A';
            dataAttributeSet.Description = "Testdescription";
            //dataAttributeSet.Is_exlusive = DataAttributeSetType.Exclusive;
            dataAttributeSet.created_at = iSessionStartedAt;
            dataAttributeSet.created_by = 1;
            dataAttributeSet.modified_at = iSessionExpireddAt;
            dataAttributeSet.modified_by = 1;

            dataAttributeSet.DataAttributes = new List<features.entity.DataAttribute>();

            features.entity.DataAttribute objattribute = new features.entity.DataAttribute();
            objattribute.ID = 3;
            features.entity.DataAttribute objattribute1 = new features.entity.DataAttribute();
            objattribute1.ID = 2;
            features.entity.DataAttribute objattribute2 = new features.entity.DataAttribute();
            objattribute2.ID = 1;

            dataAttributeSet.DataAttributes.Add(objattribute);
            dataAttributeSet.DataAttributes.Add(objattribute1);
            dataAttributeSet.DataAttributes.Add(objattribute2);

            var result = await _FeatureManager.CreateDataattributeSet(dataAttributeSet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);

        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update DataattributeSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_UpdatedataattributeSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            DataAttributeSet dataAttributeSet = new DataAttributeSet();
            dataAttributeSet.ID = 10;
            dataAttributeSet.Name = "AttributeSet_" + iSessionStartedAt;
            dataAttributeSet.State = 'A';
            dataAttributeSet.Description = "Testdescription";
            //dataAttributeSet.Is_exlusive = DataAttributeSetType.Exclusive;
            dataAttributeSet.created_at = iSessionStartedAt;
            dataAttributeSet.created_by = 1;
            dataAttributeSet.modified_at = iSessionExpireddAt;
            dataAttributeSet.modified_by = 1;

            dataAttributeSet.DataAttributes = new List<features.entity.DataAttribute>();

            features.entity.DataAttribute objattribute = new features.entity.DataAttribute();
            objattribute.ID = 3;
            features.entity.DataAttribute objattribute1 = new features.entity.DataAttribute();
            objattribute1.ID = 2;
            //features.entity.DataAttribute objattribute2 = new features.entity.DataAttribute();
            //objattribute2.ID = 1;

            dataAttributeSet.DataAttributes.Add(objattribute);
            dataAttributeSet.DataAttributes.Add(objattribute1);
            // dataAttributeSet.DataAttributes.Add(objattribute2);

            var result = await _FeatureManager.UpdatedataattributeSet(dataAttributeSet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);

        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete FeatureSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_DeleteFeatureSet()
        {
            int FeatureSetid = 135;
            bool result = await _FeatureManager.DeleteFeatureSet(FeatureSetid);
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete DataAttributeSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_DeleteDataAttribute()
        {
            int dataAttributeSetID = 10;
            bool result = await _FeatureManager.DeleteDataAttribute(dataAttributeSetID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Data Attribute Set Details ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_GetDataAttributeSetDetails()
        {
            int DataAttributeSetId = 4;
            var result = await _FeatureManager.GetDataAttributeSetDetails(DataAttributeSetId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update Feature set ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_UpdateFeatureSet()
        {
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            FeatureSet featureSet = new FeatureSet();
            featureSet.FeatureSetID = 131;
            featureSet.Name = "FeatureSet_" + iSessionStartedAt;
            featureSet.description = "Test_Description";
            featureSet.State = 'A';
            featureSet.created_at = iSessionStartedAt;
            featureSet.created_by = 1;
            featureSet.modified_at = iSessionExpireddAt;
            featureSet.modified_by = 1;

            featureSet.Features = new List<features.entity.Feature>();

            //features.entity.Feature objattribute = new features.entity.Feature();
            //objattribute.Id = 3;
            features.entity.Feature objattribute1 = new features.entity.Feature();
            objattribute1.Id = 2;
            //features.entity.Feature objattribute2 = new features.entity.Feature();
            //objattribute2.Id = 1;

            //featureSet.Features.Add(objattribute);
            featureSet.Features.Add(objattribute1);
            //featureSet.Features.Add(objattribute2);

            var result = await _FeatureManager.UpdateFeatureSet(featureSet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);

        }
    }
}
