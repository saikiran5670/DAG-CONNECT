using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategory(Category category);
        Task<Category> EditCategory(Category category);
        Task<bool> DeleteCategory(int ID);
        Task<IEnumerable<Category>> GetCategoryType( string Type);
        Task<IEnumerable<CategoryList>> GetCategoryDetails();

    }
}
