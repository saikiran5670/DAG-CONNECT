using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.package.repository;

namespace net.atos.daf.ct2.package.test
{
    [TestClass]
    public class PackageManagerTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IPackageManager _packageManager;
        private readonly IPackageRepository _packageRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureRepository _featureRepository;
        private readonly IConfiguration _config;
        public PackageManagerTest()
        {
            _config = new ConfigurationBuilder()
                         .AddJsonFile("appsettings.Test.json")
                         .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _featureRepository = new FeatureRepository(_dataAccess);
            _featureManager = new FeatureManager(_featureRepository);
            _packageRepository = new PackageRepository(_dataAccess, _featureManager);

            _packageManager = new PackageManager(_packageRepository, _featureManager);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create package with feature set")]
        [TestMethod]
        public void CreatePackageWithFeatureSet()
        {

            var ObjPackage = new Package()
            {
                Code = "PKG008",
                FeatureSetID = 1,
                Name = "Standard",
                Type = "O",
                Description = "Package with default featureset",
                State = "A"
            };
            var resultPackage = _packageManager.Create(ObjPackage).Result;
            Assert.IsNotNull(resultPackage);
            Assert.IsTrue(resultPackage.Id > 0);
        }



        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update package with feature set")]
        [TestMethod]
        public void UpdatePackageWithFeatureSet()
        {

            var ObjPackage = new Package()
            {
                Id = 76,
                Code = "PKG008",
                FeatureSetID = 4,
                State = "A",
                Name = "Standard Update",
                Type = "V",
                Description = "Package with default featureset",

            };
            var resultPackage = _packageManager.Update(ObjPackage).Result;
            Assert.IsNotNull(resultPackage);

        }



        [TestMethod]
        public void GetPackage_Manager()
        {
            var packageFilter = new PackageFilter() { State = "I" };
            var result = _packageManager.Get(packageFilter).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result != null);
        }
        [TestMethod]
        public void ImportPackage()
        {
            using (StreamReader r = new StreamReader("package.json"))
            {
                string json = r.ReadToEnd();
                var packages = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageMaster>(json);
                var result = _packageManager.Import(packages.Packages);
            }

        }
        [TestMethod]
        public void DeletePackage()
        {
            var result = _packageManager.Delete(2).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void UpdatePackageStatus()

        {
            var package = new Package() { Id = 75, State = "I" };
            var result = _packageManager.UpdatePackageState(package).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result != null);
        }



    }
}
