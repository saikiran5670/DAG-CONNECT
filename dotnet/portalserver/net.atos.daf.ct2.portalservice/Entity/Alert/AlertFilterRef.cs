using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertFilterRef
    {
        //public int Id { get; set; }

        //public int AlertId { get; set; }

        public int AlertUrgencyLevelId { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Filter type should be 1 character")]
        public string FilterType { get; set; }

        public double ThresholdValue { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Unit type should be 1 character")]
        //[AlertUnitTypeCheck(ErrorMessage = "Unit Type is invalid")]
        public string UnitType { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Landmark type should be 1 character")]
        public string LandmarkType { get; set; }

        public int RefId { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Position type should be 1 character")]
        public string PositionType { get; set; }

        public bool[] DayType { get; set; } = new bool[7];
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Period type should be 1 character")]
        public string PeriodType { get; set; }

        public long FilterStartDate { get; set; }

        public long FilterEndDate { get; set; }

        //public string State { get; set; }

        //public long CreatedAt { get; set; }

        //public long ModifiedAt { get; set; }
    }
    public class AlertFilterRefEdit : AlertFilterRef
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Alert State should be 1 character")]
        public string State { get; set; }
        //public long ModifiedAt { get; set; }
    }
}
