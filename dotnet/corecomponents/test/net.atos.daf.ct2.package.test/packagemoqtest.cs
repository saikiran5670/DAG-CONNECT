using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.package.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.package.test
{
    [TestClass]
    public class packagemoqtest
    {

        Mock<IPackageRepository> _iPackageRepository;
        Mock<IFeatureManager> _featureManager;
        PackageManager _packageManager;
        public packagemoqtest()
        {
            _iPackageRepository = new Mock<IPackageRepository>();
            _featureManager = new Mock<IFeatureManager>();
            _packageManager = new PackageManager(_iPackageRepository.Object, _featureManager.Object);
        } 
        
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create package")]
        [TestMethod]
        public async Task CreateTest()
        {            
            Package package = new Package();
                 package.Code = "PKG008";
               package.FeatureSetID = 1;
                package.Name = "Standard";
                package.Type = "O";
                package.Description = "Package with default featureset";
                package.State = "A";
            Package actual =new Package();
            _iPackageRepository.Setup(s=>s.Create(It.IsAny<Package>())).ReturnsAsync(actual);
            var result = await _packageManager.Create(package);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Import package")]
        [TestMethod]
        public async Task ImportTest()
        {            
            List<Package> packageList = new List<Package>();
                 //package.Code = "PKG008";
              // package.FeatureSetID = 1;
              //  package.Name = "Standard";
              //  package.Type = "O";
               // package.Description = "Package with default featureset";
              //  package.State = "A";
            List<Package> actual =new List<Package>();
            _iPackageRepository.Setup(s=>s.Import(It.IsAny<List<Package>>())).ReturnsAsync(actual);
            var result = await _packageManager.Import(packageList);
            Assert.AreEqual(result, actual);
        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete package")]
        [TestMethod]
        public async Task DeleteTest()
        {            
           int packageId = 34;
                 //package.Code = "PKG008";
              // package.FeatureSetID = 1;
              //  package.Name = "Standard";
              //  package.Type = "O";
               // package.Description = "Package with default featureset";
              //  package.State = "A";
        
            _iPackageRepository.Setup(s=>s.Delete(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _packageManager.Delete(packageId);
            Assert.AreEqual(result, true);
        }
         [TestCategory("Unit-Test-Case")]
        [Description("Test for Update package")]
        [TestMethod]
        public async Task UpdateTest()
        {            
            Package package = new Package();
                 package.Code = "PKG008";
               package.FeatureSetID = 1;
                package.Name = "Standard";
                package.Type = "O";
                package.Description = "Package with default featureset";
                package.State = "A";
            Package actual =new Package();
            _iPackageRepository.Setup(s=>s.Update(It.IsAny<Package>())).ReturnsAsync(actual);
            var result = await _packageManager.Update(package);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get package")]
        [TestMethod]
        public async Task GetTest()
        {            
            PackageFilter packageFilter = new PackageFilter();
               //  package.Code = "PKG008";
              // package.FeatureSetID = 1;
               // package.Name = "Standard";
               // package.Type = "O";
              //  package.Description = "Package with default featureset";
              //  package.State = "A";
            var actual =new List<Package>();
            _iPackageRepository.Setup(s=>s.Get(It.IsAny<PackageFilter>())).ReturnsAsync(actual);
            var result = await _packageManager.Get(packageFilter);
            Assert.AreEqual(result, actual);
        }

    }
}
