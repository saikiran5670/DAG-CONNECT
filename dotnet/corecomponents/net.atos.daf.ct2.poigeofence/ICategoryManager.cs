using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public interface ICategoryManager
    {
        Task<Category> AddCategory(Category category);
        Task<Category> EditCategory(Category category);
        Task<bool> DeleteCategory(int ID);
        Task<IEnumerable<Category>> GetCategory( string type);
        Task<IEnumerable<CategoryList>> GetCategoryDetails();
    }
}
