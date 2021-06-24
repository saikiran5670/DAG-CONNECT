
using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class ReportUserPreferenceCreateRequest
    {
        public int AccountId { get; set; }
        public int ReportId { get; set; }
        public int OrganizationId { get; set; }

        public List<UserPreferenceAttribute> Attributes { get; set; }
    }
    public class UserPreferenceAttribute
    {
        public int DataAttributeId { get; set; }
        public char State { get; set; }
        public char Type { get; set; }
        public char? ChartType { get; set; }
        public string ThresholdType { get; set; }
        public long ThresholdValue { get; set; }
    }
}
