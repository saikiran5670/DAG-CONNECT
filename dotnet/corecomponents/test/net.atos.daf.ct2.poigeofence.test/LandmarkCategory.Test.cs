using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.poigeofence.test
{

    [TestClass]
    public class LandmarkCategory
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryManager _categoryManager;

        public LandmarkCategory()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _categoryRepository = new CategoryRepository(_dataAccess);
            _categoryManager = new CategoryManager(_categoryRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Add Category")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void AddCategory()
        {
            Category objCategory = new Category();
            objCategory.Id = 0;
            objCategory.Organization_Id = 184;
            objCategory.Name = "TestName";
            objCategory.Type = "C";
            objCategory.IconName = "Test";
            objCategory.State = "D";
            objCategory.Parent_Id = 0;
            objCategory.Created_At = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString());
            objCategory.Created_By = 1;

            var categoryID = _categoryManager.AddCategory(objCategory).Result;
            Assert.IsNotNull(categoryID);
            Assert.IsTrue(categoryID.Id > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Edit Category")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void EditCategory()
        {
            Category objCategory = new Category();
            objCategory.Id = 20;
            objCategory.Name = "TestNameEdit";
            objCategory.IconName = "updateicon";
            objCategory.Modified_At = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString());
            objCategory.Modified_By = 1;

            var categoryID = _categoryManager.EditCategory(objCategory).Result;
            Assert.IsNotNull(categoryID);
            Assert.IsTrue(categoryID.Id > 0);

        }

        //[TestCategory("Unit-Test-Case")]
        //[Description("Test for Delete Category")]
        //[TestMethod]
        //[Timeout(TestTimeout.Infinite)]
        //public void DeleteCategory()
        //{
        //    int CategoryId = 14;
        //    var result = _categoryManager.DeleteCategory(CategoryId).Result;
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result);

        //}

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Category Type")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCategory()
        {
            string Type = "C";
            int OrganizationId = 10;
            var result = _categoryManager.GetCategory(Type, OrganizationId).Result;
            Assert.IsNotNull(result);


        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Category Details")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCategoryDetails()
        {
            var result = _categoryManager.GetCategoryDetails().Result;
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Category Wise POI")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCategoryWisePOI()
        {
            int OrganizationId = 100;
            var result =  _categoryManager.GetCategoryWisePOI(OrganizationId).Result;
            Assert.IsTrue(result.Count > 0);
        }


    }
}
