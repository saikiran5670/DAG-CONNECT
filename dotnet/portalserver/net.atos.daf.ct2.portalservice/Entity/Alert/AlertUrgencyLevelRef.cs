using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.CustomValidators.Alert;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertUrgencyLevelRefBase
    {
        [Required(ErrorMessage = "Urgency level type is required.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Urgency level type should be 1 character.")]
        [UrgencyLevelCheck(ErrorMessage = "Urgency level is invalid.")]
        public string UrgencyLevelType { get; set; }

        public string ThresholdValue { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Unit type should be 1 character.")]
        //[AlertUnitTypeCheck(ErrorMessage = "Unit Type is invalid")]
        public string UnitType { get; set; }

        public bool[] DayType { get; set; } = new bool[7];
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Period type should be 1 character.")]
        public string PeriodType { get; set; }

        public long UrgencylevelStartDate { get; set; }

        public long UrgencylevelEndDate { get; set; }

    }
    public class AlertUrgencyLevelRef : AlertUrgencyLevelRefBase
    {
        public List<AlertFilterRef> AlertFilterRefs { get; set; } = new List<AlertFilterRef>();
        //[MaxLength(4, ErrorMessage = "Maximum 4 custom period user can add per day.")]
        public List<AlertTimingDetail> AlertTimingDetails { get; set; } = new List<AlertTimingDetail>();

    }
    public class AlertUrgencyLevelRefEdit : AlertUrgencyLevelRefBase
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        //public string State { get; set; }
        //public long ModifiedAt { get; set; }
        public List<AlertFilterRefEdit> AlertFilterRefs { get; set; } = new List<AlertFilterRefEdit>();
        //[MaxLength(4, ErrorMessage = "Maximum 4 custom period user can add per day.")]
        public List<AlertTimingDetail> AlertTimingDetails { get; set; } = new List<AlertTimingDetail>();

    }
}
