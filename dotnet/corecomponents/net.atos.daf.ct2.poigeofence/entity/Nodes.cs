using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class Nodes
    {
        public int Id { get; set; }
        public int Landmark_Id { get; set; }
        public int Seq_No { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public char State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }
}
