using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence
{
    public interface ICategoryManager
    {
        Task<Category> AddCategory(Category category);
        Task<Category> EditCategory(Category category);
        Task<CategoryID> DeleteCategory(int id);
        Task<IEnumerable<Category>> GetCategory(string type, int organizationId);
        Task<IEnumerable<CategoryList>> GetCategoryDetails();
        Task<Category_SubCategory_ID_Class> BulkDeleteCategory(DeleteCategoryclass deleteCategoryclass);

        Task<List<CategoryWisePOI>> GetCategoryWisePOI(int organizationId);
    }
}
