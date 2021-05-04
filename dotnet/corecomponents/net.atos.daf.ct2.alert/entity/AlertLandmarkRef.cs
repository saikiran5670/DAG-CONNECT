using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class AlertLandmarkRef
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        public string LandmarkType { get; set; }
        public int RefId { get; set; }
        public decimal Distance { get; set; }
        public string UnitType { get; set; }
        public string State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
