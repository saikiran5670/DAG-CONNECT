using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class CategoryList
    {
       
            public int parent_id { get; set; }
            public int subcategory_id { get; set; }
            public string IconName { get; set; }
            public byte[] Icon { get; set; }
            public string ParentCategory { get; set; }
            public string SubCategory { get; set; }
            public int No_of_POI { get; set; }
            public int No_of_Geofence { get; set; }
            public List<CategoryList> CategoryLists { get; set; }

        
    }

  
}
