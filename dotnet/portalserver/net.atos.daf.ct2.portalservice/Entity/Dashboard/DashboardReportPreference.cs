using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Dashboard
{
    public class DashboardUserPreferenceCreateRequest
    {
        [Required]
        public int ReportId { get; set; }
        public List<DashboardUserPreferenceAttribute> Attributes { get; set; }
    }

    public class DashboardUserPreferenceAttribute
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
        public double ThresholdValue { get; set; }
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
        Donut = 'D',
        Line = 'L',
        Pie = 'P'
    }

    public enum ReportPreferenceThresholdType
    {
        Lower = 'L',
        Upper = 'U'
    }
}

