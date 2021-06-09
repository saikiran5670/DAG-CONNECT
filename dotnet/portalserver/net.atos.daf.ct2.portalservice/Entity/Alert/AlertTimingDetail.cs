using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.CustomValidators.Alert;
using net.atos.daf.ct2.portalservice.CustomValidators.Common;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertTimingDetail
    {
        //public int Id { get; set; }
        [AlertTimingDetailTypeCheck(ErrorMessage ="Alert timing detail type is invalid.")]
        public string Type { get; set; }
        public int RefId { get; set; }
        public bool[] DayType { get; set; } = new bool[7];
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Period type should be 1 character")]
        public string PeriodType { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        [State]
        public string State { get; set; }
    }

    public class AlertTimingDetailEdit : AlertTimingDetail
    {
        public int Id { get; set; }
    }
}
