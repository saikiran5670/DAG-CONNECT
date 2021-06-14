using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class CategoryMoqTest
    {

        Mock<ICategoryRepository> _iCategoryRepository;
        CategoryManager _CategoryManager;
        public CategoryMoqTest()
        {
            _iCategoryRepository = new Mock<ICategoryRepository>();
            _CategoryManager = new CategoryManager(_iCategoryRepository.Object);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for AddCategory")]
        [TestMethod]
        public async Task AddCategoryTest()
        {            
            Category category = new Category();
            
            Category actual =new Category();
            _iCategoryRepository.Setup(s=>s.AddCategory(It.IsAny<Category>())).ReturnsAsync(actual);
            var result = await _CategoryManager.AddCategory(category);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for EditCategory")]
        [TestMethod]
        public async Task EditCategoryTest()
        {            
            Category category = new Category();
            
            Category actual =new Category();
            _iCategoryRepository.Setup(s=>s.EditCategory(It.IsAny<Category>())).ReturnsAsync(actual);
            var result = await _CategoryManager.EditCategory(category);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for DeleteCategory")]
        [TestMethod]
        public async Task DeleteCategoryTest()
        {            
           int ID=22;
            
            CategoryID actual =new CategoryID();
            _iCategoryRepository.Setup(s=>s.DeleteCategory(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _CategoryManager.DeleteCategory(ID);
            Assert.AreEqual(result, actual);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetCategory")]
        [TestMethod]
        public async Task GetCategoryTest()
        {            
          string Type="A";
           int OrganizationId=23;
            
            var actual =new List<Category>(){new Category(){ Id=1 }};
            _iCategoryRepository.Setup(s =>s.GetCategoryType(It.IsAny<string>() ,It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _CategoryManager.GetCategory(Type, OrganizationId);
            Assert.AreEqual(result, actual);
        }

         [TestCategory("Unit-Test-Case")]
        [Description("Test for BulkDeleteCategory")]
        [TestMethod]
        public async Task BulkDeleteCategoryTest()
        {            
            DeleteCategoryclass deleteCategoryclass = new DeleteCategoryclass();
            
            Category_SubCategory_ID_Class actual =new Category_SubCategory_ID_Class();
            _iCategoryRepository.Setup(s=>s.BulkDeleteCategory(It.IsAny<DeleteCategoryclass>())).ReturnsAsync(actual);
            var result = await _CategoryManager.BulkDeleteCategory(deleteCategoryclass);
            Assert.AreEqual(result, actual);
        }

    }
}
