using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.CustomValidators.Common;

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
        [Required]
        public string State { get; set; }
        [Required]
        [StringLength(1, MinimumLength = 1)]
        public string PreferenceType { get; set; }
        [StringLength(1, MinimumLength = 0)]
        public string ChartType { get; set; }
        [StringLength(1, MinimumLength = 0)]
        public string ThresholdType { get; set; }
        public long ThresholdValue { get; set; }
    }

    public enum ReportPreferenceState
    {
        Active = 'A',
        Inactive = 'I'
    }

    public enum ReportPreferenceType
    {
        Data = 'D',
        Chart = 'C'
    }

    public enum ReportPreferenceChartType
    {
        Bar = 'B',
        Donut_or_Pie = 'D',
        Line = 'L'
    }

    public enum ReportPreferenceThresholdType
    {
        Lower = 'L',
        Upper = 'U'
    }
}