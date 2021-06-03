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
    public class PackageRepositoryTest
    {

        private readonly IDataAccess _dataAccess;
        private readonly IPackageRepository _packageRepository;
        private readonly IConfiguration _config;
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureRepository _featureRepository;


        public PackageRepositoryTest()
        {
            _config = new ConfigurationBuilder()
                         .AddJsonFile("appsettings.Test.json")
                         .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _featureManager = new FeatureManager(_featureRepository);
            _packageRepository = new PackageRepository(_dataAccess, _featureManager);

        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create package with feature set")]
        [TestMethod]
        public void CreatePackageWithFeatureSet()
        {

            var ObjPackage = new Package()
            {
                Code = "PKG001",
                // FeatureSet = new features.entity.FeatureSet() { FeatureSetID = 5 },
                FeatureSetID = 1,
                State = "A",
                Name = "Standard",
                Type = "V",
                Description = "Package with default featureset",
                // StartDate = Convert.ToDateTime("2019-02-02T12:34:56"),
                // EndDate = Convert.ToDateTime("2019-02-02T12:34:56")

            };
            var resultPackage = _packageRepository.Create(ObjPackage).Result;
            Assert.IsNotNull(resultPackage);
            Assert.IsTrue(resultPackage.Id > 0);
        }
    }
}
