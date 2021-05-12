using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Corridor
{

    public class Nodepoint
    {
        public int Id { get; set; }
        [Required]
        public int LandmarkId { get; set; }
        [Column("trip_id")]
        [Required]
        public string TripId { get; set; }

        [Column("seq_no")]
        public int SequenceNumber { get; set; }
        [Column("latitude")]
        [Required]
        public double Latitude { get; set; }
        [Column("longitude")]
        [Required]
        public double Longitude { get; set; }
        public string State { get; set; }

        [Column("address")]
        public string Address { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class ExistingTrip
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int LandmarkId { get; set; }
        [Required]
        [Column("trip_id")]
        public string TripId { get; set; }
        [Column("start_date")]
        [Required]
        public double StartDate { get; set; }
        [Column("end_date")]
        [Required]
        public double EndDate { get; set; }
        [Column("driver_id1")]
        public string DriverId1 { get; set; }
        [Column("driver_id2")]
        public string DriverId2 { get; set; }
        [Required]
        public double StartLatitude { get; set; }
        [Required]
        public double StartLongitude { get; set; }
        
        public string StartPosition { get; set; }
        [Required]
        public double EndLatitude { get; set; }
        [Required]
        public double EndLongitude { get; set; }
        
        public string EndPosition { get; set; }
        [Required]
        public int Distance { get; set; }
        [Required]
        public List<Nodepoint> NodePoints { get; set; }
    }
    public class ExistingTripCorridor
    {

        [Required]
        public int Id { get; set; }
        [Required]
        public int? OrganizationId { get; set; }
        
        public string CorridorType { get; set; }
        [Required]
        public string CorridorLabel { get; set; }
        [Required]
        public int Width { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public string Description { get; set; }
        
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        [Required]
        public double StartLatitude { get; set; }
        [Required]
        public double StartLongitude { get; set; }
        [Required]
        public double Distance { get; set; }
        public string State { get; set; }
        [Required]
        public List<ExistingTrip> ExistingTrips { get; set; }
    }
}
