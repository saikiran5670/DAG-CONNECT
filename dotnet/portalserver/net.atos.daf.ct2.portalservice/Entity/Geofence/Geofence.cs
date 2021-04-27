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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public int TripId { get; set; }
        public int CreatedBy { get; set; }
        public List<Nodes> Nodes { get; set; }
    }
    public class Nodes
    {
        public int Id { get; set; }
        public int LandmarkId { get; set; }
        public int SeqNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CreatedBy { get; set; }
    }

    public class CircularGeofence
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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required]
        public double Distance { get; set; }
        public int TripId { get; set; }
        public int CreatedBy { get; set; }
    }

    public class GeofenceUpdateEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public int ModifiedBy { get; set; }
    }
}