using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class CategoryList
    {

        public int Parent_id { get; set; }
        public int Subcategory_id { get; set; }
        public int IconId { get; set; }
        public byte[] Icon { get; set; }
        public string ParentCategory { get; set; }
        public string SubCategory { get; set; }
        public int No_of_POI { get; set; }
        public int No_of_Geofence { get; set; }
        public string Description { get; set; }
        public long Created_at { get; set; }
        public string Icon_Name { get; set; }
        public int Organization_Id { get; set; }

        public List<CategoryList> CategoryLists { get; set; }


    }


}
