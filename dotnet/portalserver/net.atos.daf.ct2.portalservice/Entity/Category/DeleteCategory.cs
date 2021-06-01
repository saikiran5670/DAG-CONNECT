using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Category
{
    public class DeleteCategory
    {
        // public int[] Ids { get; set; }
        public List<Category_SubCategory_ID> category_SubCategory_s { get; set; }

    }

    public class Category_SubCategory_ID
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
}
