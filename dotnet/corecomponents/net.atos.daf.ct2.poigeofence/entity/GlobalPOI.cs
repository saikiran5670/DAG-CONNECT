namespace net.atos.daf.ct2.poigeofence.entity
{
    public class POIEntityResponse
    {
        public int GlobalPOIId { get; set; }
        public string POIName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string City { get; set; }
    }

    public class POIEntityRequest
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
}
