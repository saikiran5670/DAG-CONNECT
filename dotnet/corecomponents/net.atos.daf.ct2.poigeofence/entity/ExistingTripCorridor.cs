using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.atos.daf.ct2.poigeofence.entity
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Nodepoint
    {
        public int Id { get; set; }
       
        public int LandmarkId { get; set; }
        [Column("trip_id")]
        public string TripId { get; set; }

        [Column("seq_no")]
        public int SequenceNumber { get; set; }
        [Column("latitude")]
        public double Latitude { get; set; }
        [Column("longitude")]
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

        public int Id { get; set; }
        public int LandmarkId { get; set; }

        [Column("trip_id")]
        public string TripId { get; set; } 
        [Column("start_date")]
        public double StartDate { get; set; }
        [Column("end_date")]
        public double EndDate { get; set; }
        [Column("driver_id1")]
        public string DriverId1 { get; set; }
        [Column("driver_id2")]
        public string DriverId2 { get; set; }
        public double StartLatitude { get; set; }

        public double StartLongitude { get; set; }
        public string StartPosition { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
        public string EndPosition { get; set; }
        public int Distance { get; set; }

        public List<Nodepoint> NodePoints { get; set; }
    } 
    public class ExistingTripCorridor
    {
          

        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public string CorridorType { get; set; }
        public string CorridorLabel { get; set; }//name
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
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public double Distance { get; set; }
        public string State { get; set; }   
        public List<ExistingTrip> ExistingTrips { get; set; }
    }
}
