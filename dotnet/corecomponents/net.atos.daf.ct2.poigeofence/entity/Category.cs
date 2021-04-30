﻿using net.atos.daf.ct2.poigeofence.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class Category
    {
        public int Id { get; set; }
        public int ? Organization_Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public string Type { get; set; }
        public int Parent_Id { get; set; }
        public string State { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }
        public byte[] icon { get; set; }
        public string Description { get; set; }


    }
    

    public class CategoryID
    {
        public int ID { get; set; }
    }


    public class DeleteCategoryclass
    {
        // public int[] Ids { get; set; }
        public List<Category_SubCategory_ID_Class> category_SubCategory_s { get; set; }

    }

    public class Category_SubCategory_ID_Class
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }



}
