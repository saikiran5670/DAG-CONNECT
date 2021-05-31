namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class POIResponse
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string Name { get; set; }
        //public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        //public double Distance { get; set; }
        public string State { get; set; }
        //public int TripId { get; set; }
        public long CreatedAt { get; set; }
        public byte[] Icon { get; set; }
        //public int CreatedBy { get; set; }
        //public long ModifiedAt { get; set; }
        //public int ModifiedBy { get; set; }
    }
}
