using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class Nodes
    {
        public int Id { get; set; }
        public int LandmarkId { get; set; }
        public int SeqNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public string Message { get; set; }
        public bool IsFailed { get; set; }
        public bool IsAdded { get; set; }
        public string Address { get; set; }
        public string TripId { get; set; }
    }
}
