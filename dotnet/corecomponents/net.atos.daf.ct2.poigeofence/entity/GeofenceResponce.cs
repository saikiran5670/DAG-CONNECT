namespace net.atos.daf.ct2.poigeofence.entity
{
    public class GeofenceEntityResponce
    {
        public string geofenceName { get; set; }
        public string category { get; set; }
        public string subCategory { get; set; }
        public int geofenceID { get; set; }
        public string type { get; set; }
        public int categoryID { get; set; }
        public int subcategoryId { get; set; }

    }
    public class GeofenceEntityRequest
    {
        public int organization_id { get; set; }
        public int category_id { get; set; }
        public int sub_category_id { get; set; }
        public int roleIdlevel { get; set; }
    }
}
