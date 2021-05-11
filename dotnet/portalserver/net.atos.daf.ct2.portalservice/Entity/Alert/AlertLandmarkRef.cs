using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertLandmarkRef
    {
        //public int Id { get; set; }
        //public int AlertId { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Landmark type should be 1 character")]
        public string LandmarkType { get; set; }
        public int RefId { get; set; }
        public double Distance { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Unit type should be 1 character")]
        public string UnitType { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
    }
    public class AlertLandmarkRefEdit : AlertLandmarkRef
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Alert State should be 1 character")]
        public string State { get; set; }
        //public long ModifiedAt { get; set; }
    }
}
