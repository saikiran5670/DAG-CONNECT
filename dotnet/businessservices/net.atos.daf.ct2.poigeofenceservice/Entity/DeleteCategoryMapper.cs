﻿
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofenceservice.entity
{
    public class DeleteCategoryMapper
    {

        public DeleteCategoryclass ToTranslationDeleteEntity(DeleteRequest request)
        {
            DeleteCategoryclass obj = new DeleteCategoryclass();
            // obj.id = request.Id;
            obj.category_SubCategory_s = new List<Category_SubCategory_ID_Class>();


            if (request.CatSubCatIDList != null)
            {
                foreach (var item in request.CatSubCatIDList)
                {
                    var trans = new Category_SubCategory_ID_Class();
                    trans.CategoryId = item.CategoryId;
                    trans.SubCategoryId = item.SubCategoryId;
                   
                    obj.category_SubCategory_s.Add(trans);
                }

            }
           
            return obj;
        }
    }
}
