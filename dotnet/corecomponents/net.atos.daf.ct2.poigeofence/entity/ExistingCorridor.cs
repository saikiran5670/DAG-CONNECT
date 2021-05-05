using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.atos.daf.ct2.poigeofence.entity
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    //landmark_id
    public class Nodepoint
    {
        [Column("seq_no")]
        public int SequenceNumber { get; set; }
        [Column("latitude")]
        public double Latitude { get; set; }
        [Column("longitude")]
        public double Longitude { get; set; }

        [Column("address")]
        public string Address { get; set; }
    }

    public class ExistingTrip
    {
        [Column("trip_id")]
        public int TripId { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public string StartPoint { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
        public string EndPoint { get; set; }
        public List<Nodepoint> NodePoints { get; set; }
    } 
    public class ExistingCorridor
    {
        public string CorridorType { get; set; }
        public string CorridorLabel { get; set; }
        [Column("distance")]
        public string Distance { get; set; }
        public List<ExistingTrip> ExistingTrips { get; set; }
    }
}
