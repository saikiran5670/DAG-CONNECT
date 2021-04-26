using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public class CategoryManager : ICategoryManager
    {
            private readonly ICategoryRepository _categoryRepository;
            public CategoryManager( ICategoryRepository categoryRepository)
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

        public async Task<bool> DeleteCategory(int ID)
        {
            return await _categoryRepository.DeleteCategory(ID);
        }

        public async Task<IEnumerable<Category>> GetCategory( string Type)
        {
            return await _categoryRepository.GetCategoryType(Type);
        }
    }
}
