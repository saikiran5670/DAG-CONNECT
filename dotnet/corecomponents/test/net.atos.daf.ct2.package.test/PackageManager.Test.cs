using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.package.ENUM;
using net.atos.daf.ct2.package.repository;
using System;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.repository;
using System.IO;
using System.Collections.Generic;

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
               // Default = PackageDefault.True,
                FeatureSetID = 1,
                Status = PackageStatus.Active,
                Name = "Standard",
                Type = "O",
                Description = "Package with default featureset",
             //   StartDate = Convert.ToDateTime("2019-02-02T12:34:56"),
              //  EndDate = Convert.ToDateTime("2019-02-02T12:34:56")

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
                Id =3,
                Code = "PKG011",
                //Default = PackageDefault.True,
                //  FeatureSet = new features.entity.FeatureSet() { FeatureSetID = 5},

                FeatureSetID = 4,
                Status = PackageStatus.Active,
                // Is_Active = true,
                Name = "Standard",
                Type = "Organization",
                Description = "Package with default featureset",
              //  StartDate = Convert.ToDateTime("2019-02-02T12:34:56"),
              //  EndDate = Convert.ToDateTime("2019-02-02T12:34:56")

            };
            var resultPackage = _packageManager.Update(ObjPackage).Result;
            Assert.IsNotNull(resultPackage);

        }



        [TestMethod]
        public void GetPackage_Manager()
        {
            var packageFilter = new PackageFilter() /*{ Id = 2 }*/;
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
                var result = _packageManager.Import(packages.packages);
            }

        }
        [TestMethod]
        public void DeletePackage()
        {
            var result = _packageManager.Delete(1).Result;
            Console.WriteLine(result);
            Assert.IsTrue(result);
        }

    }
}
