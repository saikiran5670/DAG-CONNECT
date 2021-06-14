using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features.entity;
//using Xunit;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.repository;
using System.Threading.Tasks;
//using NUnit.Framework;

namespace net.atos.daf.ct2.feature.test
{
    [TestClass]
    public class FeatureTestMoq
    {

        Mock<IFeatureRepository> _iFeatuteRepository;
        FeatureManager _featureManager;
        public FeatureTestMoq()
        {
            _iFeatuteRepository = new Mock<IFeatureRepository>();
            _featureManager = new FeatureManager(_iFeatuteRepository.Object);
        }   

         [TestCategory("Unit-Test-Case")]
        [Description("Test for CreateFeatureSet")]
        [TestMethod]
        public async Task CreateFeatureSetTest()
        {            
            FeatureSet featureSet = new FeatureSet();
           featureSet.Description = null;
            featureSet.State = 'A';
            //featureSet.Created_at = iSessionStartedAt;
            featureSet.Created_by = 1;
           // featureSet.Modified_at = iSessionExpireddAt;
            featureSet.Modified_by = 1;
            FeatureSet actual =new FeatureSet();
            _iFeatuteRepository.Setup(s=>s.CreateFeatureSet(It.IsAny<FeatureSet>())).ReturnsAsync(actual);
            var result = await _featureManager.CreateFeatureSet(featureSet);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for CreateDataattributeSet")]
        [TestMethod] 
        public async Task CreateDataattributeSettTest()
        {            
            DataAttributeSet dataAttributeSet = new DataAttributeSet();
           dataAttributeSet.Description = null;
            dataAttributeSet.State = 'A';
            //featureSet.Created_at = iSessionStartedAt;
            dataAttributeSet.Created_by = 1;
           // featureSet.Modified_at = iSessionExpireddAt;
            dataAttributeSet.Modified_by = 1;
            DataAttributeSet actual =new DataAttributeSet();
            _iFeatuteRepository.Setup(s=>s.CreateDataattributeSet(It.IsAny<DataAttributeSet>())).ReturnsAsync(actual);
            var result = await _featureManager.CreateDataattributeSet(dataAttributeSet);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdatedataattributeSet")]
        [TestMethod] 
        public async Task UpdatedataattributeSettTest()
        {            
            DataAttributeSet dataAttributeSet = new DataAttributeSet();
           dataAttributeSet.Description = null;
            dataAttributeSet.State = 'A';
            //featureSet.Created_at = iSessionStartedAt;
            dataAttributeSet.Created_by = 1;
           // featureSet.Modified_at = iSessionExpireddAt;
            dataAttributeSet.Modified_by = 1;
            DataAttributeSet actual =new DataAttributeSet();
            _iFeatuteRepository.Setup(s=>s.UpdatedataattributeSet(It.IsAny<DataAttributeSet>())).ReturnsAsync(actual);
            var result = await _featureManager.UpdatedataattributeSet(dataAttributeSet);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteFeatureSet")]
        [TestMethod] 
        public async Task DeleteFeatureSettTest()
        {            
           int FeatureSetId = 32;
           // DataAttributeSet actual =new DataAttributeSet();
            _iFeatuteRepository.Setup(s=>s.DeleteFeatureSet(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _featureManager.DeleteFeatureSet(FeatureSetId);
            Assert.AreEqual(result, true);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteDataAttribute")]
        [TestMethod] 
        public async Task DeleteDataAttributetTest()
        {            
           int dataAttributeSetID = 32;
           // DataAttributeSet actual =new DataAttributeSet();
            _iFeatuteRepository.Setup(s=>s.DeleteDataAttribute(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _featureManager.DeleteDataAttribute(dataAttributeSetID);
            Assert.AreEqual(result, true);
        }

         [TestCategory("Unit-Test-Case")]
        [Description("Test for GetDataAttributeSetDetails")]
        [TestMethod] 
        public async Task GetDataAttributeSetDetailsTest()
        {            
           int dataAttributeSetID = 32;
           List<DataAttributeSet> actual =new List<DataAttributeSet>();
            _iFeatuteRepository.Setup(s=>s.GetDataAttributeSetDetails(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _featureManager.GetDataAttributeSetDetails(dataAttributeSetID);
            Assert.AreEqual(result, actual);
        }

         [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateFeatureSet")]
        [TestMethod]
        public async Task UpdateFeatureSetTest()
        {            
            FeatureSet featureSet = new FeatureSet();
           featureSet.Description = null;
            featureSet.State = 'A';
            //featureSet.Created_at = iSessionStartedAt;
            featureSet.Created_by = 1;
           // featureSet.Modified_at = iSessionExpireddAt;
            featureSet.Modified_by = 1;
            FeatureSet actual =new FeatureSet();
            _iFeatuteRepository.Setup(s=>s.UpdateFeatureSet(It.IsAny<FeatureSet>())).ReturnsAsync(actual);
            var result = await _featureManager.UpdateFeatureSet(featureSet);
            Assert.AreEqual(result, actual);
        }
    }
}
