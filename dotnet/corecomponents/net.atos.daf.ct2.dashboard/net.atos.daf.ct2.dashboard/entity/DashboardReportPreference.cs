using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class DashboardUserPreferenceCreateRequest
    {
        public int AccountId { get; set; }
        public int ReportId { get; set; }
        public int OrganizationId { get; set; }
        public List<DashboardUserPreferenceAttribute> Attributes { get; set; }
    }
    public class DashboardUserPreferenceAttribute
    {
        public int DataAttributeId { get; set; }
        public ReportUserPreferenceState State { get; set; }
        public ReportPreferenceType Type { get; set; }
        public ReportPreferenceChartType? ChartType { get; set; }
        public ReportPreferenceThresholdType? ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
    }

    public enum ReportUserPreferenceState
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
