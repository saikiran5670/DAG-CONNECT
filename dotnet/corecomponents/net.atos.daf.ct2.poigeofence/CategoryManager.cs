using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence
{
    public class CategoryManager : ICategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryManager(ICategoryRepository categoryRepository)
        {

            _categoryRepository = categoryRepository;
        }
        public async Task<Category> AddCategory(Category category)
        {
            return await _categoryRepository.AddCategory(category);
        }

        public async Task<Category> EditCategory(Category category)
        {
            return await _categoryRepository.EditCategory(category);
        }

        public async Task<CategoryID> DeleteCategory(int id)
        {
            return await _categoryRepository.DeleteCategory(id);
        }

        public async Task<IEnumerable<Category>> GetCategory(string type, int organizationId)
        {
            return await _categoryRepository.GetCategoryType(type, organizationId);
        }

        public async Task<IEnumerable<CategoryList>> GetCategoryDetails()
        {
            return await _categoryRepository.GetCategoryDetails();
        }

        public async Task<Category_SubCategory_ID_Class> BulkDeleteCategory(DeleteCategoryclass deleteCategoryclass)
        {
            return await _categoryRepository.BulkDeleteCategory(deleteCategoryclass);
        }

        public async Task<List<CategoryWisePOI>> GetCategoryWisePOI(int organizationId)
        {
            return await _categoryRepository.GetCategoryWisePOI(organizationId);
        }
    }
}
