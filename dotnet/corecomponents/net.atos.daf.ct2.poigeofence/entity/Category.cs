﻿using net.atos.daf.ct2.poigeofence.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class Category
    {
        public int Id { get; set; }
        public int Organization_Id { get; set; }
        public string Name { get; set; }
        public int Icon_Id { get; set; }
        public string Type { get; set; }
        public int Parent_Id { get; set; }
        public string State { get; set; }
        public long Created_At { get; set; }
        public int Created_By { get; set; }
        public long Modified_At { get; set; }
        public int Modified_By { get; set; }

    }

    //public class CategoryList
    //{
    //    public List<Category> Categories { get; set; }
    //    public List<SubCategory> SubCategories { get; set; }

    //}
}
