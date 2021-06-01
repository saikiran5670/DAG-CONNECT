using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class LandmarkGroup
    {
        public int id { get; set; }
        public int organization_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int icon_id { get; set; }
        public string state { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public int poiCount { get; set; }
        public int geofenceCount { get; set; }
        public List<POI> PoiList { get; set; }
        public List<Geofence> GeofenceList { get; set; }
    }

    public class LandmarkgroupRef
    {
        public int id { get; set; }
        public int landmark_group_id { get; set; }
        public LandmarkType type { get; set; }
        public int ref_id { get; set; }
        public string category { get; set; }
        public string categoryname { get; set; }
        public string landmarkname { get; set; }
        public int landmarkid { get; set; }
        public string subcategoryname { get; set; }
        public byte[] icon { get; set; }
        public string address { get; set; }
        public int MyProperty { get; set; }
    }
}
