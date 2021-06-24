using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class ReportUserPreferenceCreateRequest
    {
        [Required]
        public int ReportId { get; set; }
        public List<UserPreferenceAttribute> Attributes { get; set; }
    }

    public class UserPreferenceAttribute
    {
        [Required]
        public int DataAttributeId { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string State { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string Type { get; set; }
        public string ChartType { get; set; }
        public string ThresholdType { get; set; }
        public long ThresholdValue { get; set; }
    }
}