namespace net.atos.daf.ct2.poigeofence.entity
{
    public class GeofenceEntityResponce
    {
        public string GeofenceName { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int GeofenceID { get; set; }
        public string Type { get; set; }
        public int CategoryID { get; set; }
        public int SubcategoryId { get; set; }

    }
    public class GeofenceEntityRequest
    {
        public int Organization_id { get; set; }
        public int Category_id { get; set; }
        public int Sub_category_id { get; set; }
        public int RoleIdlevel { get; set; }
    }
}
