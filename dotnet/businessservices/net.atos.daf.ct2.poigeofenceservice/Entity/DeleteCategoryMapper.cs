
using System.Collections.Generic;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofences;

namespace net.atos.daf.ct2.poigeofenceservice.entity
{
    public class DeleteCategoryMapper
    {

        public DeleteCategoryclass ToTranslationDeleteEntity(DeleteRequest request)
        {
            DeleteCategoryclass obj = new DeleteCategoryclass();
            // obj.id = request.Id;
            obj.Category_SubCategory_s = new List<Category_SubCategory_ID_Class>();


            if (request.CatSubCatIDList != null)
            {
                foreach (var item in request.CatSubCatIDList)
                {
                    var trans = new Category_SubCategory_ID_Class();
                    trans.CategoryId = item.CategoryId;
                    trans.SubCategoryId = item.SubCategoryId;

                    obj.Category_SubCategory_s.Add(trans);
                }

            }

            return obj;
        }
    }
}
