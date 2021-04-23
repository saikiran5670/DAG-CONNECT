using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Geofence
{
    public class Geofence
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Distance { get; set; }
        public int TripId { get; set; }
        public int CreatedBy { get; set; }
        public List<Nodes> Nodes { get; set; }
    }
    public class Nodes
    {
        public int Id { get; set; }
        public int LandmarkId { get; set; }
        public int SeqNo { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CreatedBy { get; set; }
    }
}
