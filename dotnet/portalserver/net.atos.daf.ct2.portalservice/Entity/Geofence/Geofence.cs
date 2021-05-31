using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Geofence
{
    public class Geofence
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        [Required]
        [RegularExpression(@"^[\s\w\p{L}\-\.]{1,100}$", ErrorMessage = "Geofenace name Allowed: a-z, A-Z, 0-9, hyphens dash, spaces, periods, international alphabets [e.g. à, è, ì, ò, ù, À, È, Ì, Ò, Ù] Not allowed: special chars[i.e. !, @, #, $, %, &, *] with maximun length 100.")]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [RegularExpression(@"^(?:[0-9][0-9]{0,4}?|100000)$", ErrorMessage = "Negative and decimal numbers are not allowed. Maximum limit for radius should be 100,000m")]
        public double Distance { get; set; }
        public int Width { get; set; }
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
        [MaxLength(100)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string TripId { get; set; }
    }

    public class CircularGeofence
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        [Required(ErrorMessage = "Geofence name is required")]
        [RegularExpression(@"^[\s\w\p{L}\-\.]{1,100}$", ErrorMessage = "Geofenace name Allowed: a-z, A-Z, 0-9, hyphens dash, spaces, periods, international alphabets [e.g. à, è, ì, ò, ù, À, È, Ì, Ò, Ù] Not allowed: special chars[i.e. !, @, #, $, %, &, *] with maximun length 100.")]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Geofence radius is required")]
        [RegularExpression(@"^(?:[1-9][0-9]{0,4}?|100000)$", ErrorMessage = "Negative and decimal numbers are not allowed. Maximum limit for radius should be 100,000m")]
        public double Distance { get; set; }
        public int Width { get; set; }
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
        public int OrganizationId { get; set; }
        public double Distance { get; set; }
    }

    public class GeofenceEntity
    {
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
    public class GeofencebyIDEntity
    {
        public int OrganizationId { get; set; }
        public int GeofenceId { get; set; }
    }
    public class GeofenceDeleteEntity
    {
        public List<int> GeofenceId { get; set; }

    }
    public class GeofenceFilter
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class DeleteGeofences
    {
        public List<int> GeofenceIds { get; set; }
        public int ModifiedBy { get; set; }
    }
}