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
    public class Featurerepositorytest
    {
        private readonly IFeatureManager _featureManager;
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly FeatureRepository _featureRepository;
        public Featurerepositorytest()
        {
            // string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";

            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();

            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _featureRepository = new FeatureRepository(_dataAccess);
            _featureManager = new FeatureManager(_featureRepository);

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
            feature.Description = null;
            feature.State = 'A';
            feature.Created_at = iSessionStartedAt;
            feature.Created_by = 1;
            feature.Modified_at = iSessionExpireddAt;
            feature.Modified_by = 1;

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

            var result = await _featureManager.CreateFeatureSet(feature);
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
            dataAttributeSet.Created_at = iSessionStartedAt;
            dataAttributeSet.Created_by = 1;
            dataAttributeSet.Modified_at = iSessionExpireddAt;
            dataAttributeSet.Modified_by = 1;

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

            var result = await _featureManager.CreateDataattributeSet(dataAttributeSet);
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
            dataAttributeSet.Created_at = iSessionStartedAt;
            dataAttributeSet.Created_by = 1;
            dataAttributeSet.Modified_at = iSessionExpireddAt;
            dataAttributeSet.Modified_by = 1;

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

            var result = await _featureManager.UpdatedataattributeSet(dataAttributeSet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);

        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete FeatureSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_DeleteFeatureSet()
        {
            int FeatureSetid = 135;
            bool result = await _featureManager.DeleteFeatureSet(FeatureSetid);
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete DataAttributeSet ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_DeleteDataAttribute()
        {
            int dataAttributeSetID = 10;
            bool result = await _featureManager.DeleteDataAttribute(dataAttributeSetID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Data Attribute Set Details ")]
        [TestMethod]
        public async Task UnT_features_FeatureManager_GetDataAttributeSetDetails()
        {
            int DataAttributeSetId = 4;
            var result = await _featureManager.GetDataAttributeSetDetails(DataAttributeSetId);
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
            featureSet.Description = "Test_Description";
            featureSet.State = 'A';
            featureSet.Created_at = iSessionStartedAt;
            featureSet.Created_by = 1;
            featureSet.Modified_at = iSessionExpireddAt;
            featureSet.Modified_by = 1;

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

            var result = await _featureManager.UpdateFeatureSet(featureSet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result != null);

        }
    }
}
