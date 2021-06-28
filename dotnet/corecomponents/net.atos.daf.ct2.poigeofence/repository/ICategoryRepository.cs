using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategory(Category category);
        Task<Category> EditCategory(Category category);
        Task<CategoryID> DeleteCategory(int id);
        Task<IEnumerable<Category>> GetCategoryType(string type, int organizationId);
        Task<IEnumerable<CategoryList>> GetCategoryDetails(int orgId);
        Task<IEnumerable<Category>> GetCategory(CategoryFilter categoryFilter);
        Task<Category_SubCategory_ID_Class> BulkDeleteCategory(DeleteCategoryclass deleteCategoryclass);
        Task<List<CategoryWisePOI>> GetCategoryWisePOI(int organizationId);

    }
}
