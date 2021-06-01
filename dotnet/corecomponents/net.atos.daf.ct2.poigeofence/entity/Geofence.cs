using System.Collections.Generic;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class Geofence
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public string State { get; set; }
        public int Width { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public List<Nodes> Nodes { get; set; }
        public bool Exists { get; set; }
        public string Message { get; set; }
        public bool IsFailed { get; set; }
        public bool IsAdded { get; set; }
    }
    public class GeofenceDeleteEntity
    {
        public List<int> GeofenceId { get; set; }
        public int ModifiedBy { get; set; }
    }
}
