using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
//using Xunit;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.features.entity;
using Dapper;
using System.Data.Common;
//using NUnit.Framework;

namespace net.atos.daf.ct2.feature.test
{
    [TestClass]
    public class featuremoqtest
    {

        private readonly FeatureRepository _featureRepository;
        private readonly Mock<IDataAccess> _dataAccessRepoMock = new Mock<IDataAccess>();

        public featuremoqtest()
        {
            _featureRepository = new FeatureRepository(_dataAccessRepoMock.Object);
        }

        [TestMethod]
        public async Task CreateFeatureSet()
        {


            // Arrange
            var mock = new Mock<FeatureRepository>();
            FeatureSet obj = new FeatureSet();
            var expectedFeatureSetID = 1;
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            //obj.FeatureSetID = 1;
            obj.Name = "Test_FeatureName";
            obj.description = "Test_description";
            obj.State = Convert.ToChar( "A");
            obj.created_at = iSessionStartedAt;
            obj.created_by = 1;
            obj.modified_at = iSessionExpireddAt;
            obj.modified_by = 1;
            // _dataAccessRepoMock.Setup(db => db.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<DynamicParameters>())).ReturnsAsync(expectedFeatureSetID);
            _dataAccessRepoMock.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(expectedFeatureSetID);
            //mock.Setup(m => m.CreateFeatureSet(obj)).ReturnsAsync(expectedFeatureSetID);


            // Act
            var featureSet = await _featureRepository.CreateFeatureSet(obj);

            // Assert

            Assert.AreEqual(featureSet.FeatureSetID, expectedFeatureSetID);

            _dataAccessRepoMock.Verify(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>()), Times.Once());

        }


        [TestMethod]
        public async Task CreateFeatureSet_CreateFeatureSetMapping()
        {
            // Arrange
            var mock = new Mock<FeatureRepository>();
            FeatureSet obj = new FeatureSet();

            var expectedFeatureSetID = 1;
            var expectedFeatureID = 3;
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            //obj.FeatureSetID = 1;
            obj.Name = "Test_FeatureName";
            obj.description = "Test_description";
            obj.State = Convert.ToChar("A");
            obj.created_at = iSessionStartedAt;
            obj.created_by = 1;
            obj.modified_at = iSessionExpireddAt;
            obj.modified_by = 1;

            obj.Features = new List<features.entity.Feature>();
            features.entity.Feature objfeature = new features.entity.Feature();
            objfeature.Id = 4;
            features.entity.Feature objfeature1 = new features.entity.Feature();
            objfeature1.Id = 2;
            features.entity.Feature objfeature2 = new features.entity.Feature();
            objfeature2.Id = 3;

            obj.Features.Add(objfeature);
            obj.Features.Add(objfeature1);
            obj.Features.Add(objfeature2);

            _dataAccessRepoMock.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(expectedFeatureSetID);
            _dataAccessRepoMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<object>())).Returns(expectedFeatureID);

            // Act
            var featureSet = await _featureRepository.CreateFeatureSet(obj);

            // Assert

            //Assert.AreEqual(featureSet.FeatureSetID, expectedFeatureSetID);

            _dataAccessRepoMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));

        }


        //[TestMethod]
        //public void ExecuteScalarAsync()
        //{

        //    const string expected = "Hello";

        //    _dataAccessRepoMock.Setup(c => c.ExecuteScalarAsync<object>(It.IsAny<string>(), null, null, null, null))
        //              .ReturnsAsync(expected);

        //    var actual = _dataAccessRepoMock.Object
        //                           .ExecuteScalarAsync<object>("")
        //                           .GetAwaiter()
        //                           .GetResult();

        //    Assert.AreEqual(actual, expected);
        //}

        [TestMethod]
        public async Task GetDataAttributeSetDetails()
        {
            try
            {
                // Arrange
                int expected = 1;
                int DataAttributeSetId = 1;
                long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                long iSessionModifydAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
                var mock = new Mock<FeatureRepository>();
                DataAttributeSet obj = new DataAttributeSet();

                List<DataAttributeSetM> objlist = new List<DataAttributeSetM>();
                DataAttributeSetM ob = new DataAttributeSetM();
                ob.id = 4;
                ob.name = "AttributeSet_1614240653811";
                ob.description = "Testdescription";
                ob.is_exlusive = true;
                ob.created_at = iSessionStartedAt;
                ob.created_by = 1;
                ob.modified_at = iSessionModifydAt;
                ob.modified_by = 1;
                objlist.Add(ob);

                //ob = new DataAttributeSet();
                //ob.ID = 2;
                //ob.Name = "DataAttribute_2";
                //ob.Description = "Test_2";
                //ob.Is_exlusive = true;
                //ob.created_at = iSessionStartedAt;
                //ob.created_by = 1;
                //ob.modified_at = iSessionModifydAt;
                //ob.modified_by = 1;
                //objlist.Add(ob);

                _dataAccessRepoMock.Setup(x => x.QueryAsync<dynamic>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(objlist);
                // Act
                dynamic result = await _featureRepository.GetDataAttributeSetDetails(DataAttributeSetId);  //Task<IEnumerable<T>>
                // Assert
                Assert.AreEqual(result.Count, expected);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public async Task CreateDataattributeSet()
        {
            // Arrange
            var mock = new Mock<FeatureRepository>();
            DataAttributeSet obj = new DataAttributeSet();
            var expectedFeatureSetID = 1;
            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));

            obj.Name = "Test_FeatureName";
            obj.Description = "Test_description";
            obj.State = Convert.ToChar("A");
            obj.created_at = iSessionStartedAt;
            obj.created_by = 1;
            obj.modified_at = iSessionExpireddAt;
            obj.modified_by = 1;


            _dataAccessRepoMock.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(expectedFeatureSetID);

            // Act
            var UpdatedDataAttributeSetId = await _featureRepository.UpdatedataattributeSet(obj);
            var dataAttributeSetID = await _featureRepository.CreateDataattributeSet(obj);

            // Assert

            Assert.AreEqual(dataAttributeSetID.ID, expectedFeatureSetID);

            _dataAccessRepoMock.Verify(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>()), Times.Once());

        }


        public class DataAttributeSetM
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool isActive { get; set; }
            public string description { get; set; }
            //public string Is_exlusive { get; set; }
            public long created_at { get; set; }
            public int created_by { get; set; }
            public long modified_at { get; set; }
            public int modified_by { get; set; }
            public List<DataAttribute> DataAttributes { get; set; }
            public bool is_exlusive { get; set; }

        }

    }
}
