using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class LandmarkGroup
    {
        public int Id { get; set; }
        public int Organization_id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Icon_id { get; set; }
        public string State { get; set; }
        public long Created_at { get; set; }
        public int Created_by { get; set; }
        public long Modified_at { get; set; }
        public int Modified_by { get; set; }
        public int PoiCount { get; set; }
        public int GeofenceCount { get; set; }
        public List<POI> PoiList { get; set; }
        public List<Geofence> GeofenceList { get; set; }
    }

    public class LandmarkgroupRef
    {
        public int Id { get; set; }
        public int Landmark_group_id { get; set; }
        public LandmarkType Type { get; set; }
        public int Ref_id { get; set; }
        public string Category { get; set; }
        public string Categoryname { get; set; }
        public string Landmarkname { get; set; }
        public int Landmarkid { get; set; }
        public string Subcategoryname { get; set; }
        public byte[] Icon { get; set; }
        public string Address { get; set; }
        public int MyProperty { get; set; }
    }
}
