namespace net.atos.daf.ct2.reports.entity
{
    public class UserPreferenceReportDataColumn
    {
        public int DataAtrributeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public int ReportPreferenceId { get; set; }
        public string ChartType { get; set; }
        public string ReportPreferenceType { get; set; }
        public string ThresholdType { get; set; }
        public long ThresholdValue { get; set; }
    }

    public class ReportUserPreference
    {
        public int DataAttributeId { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public string ChartType { get; set; }
        public string ReportPreferenceType { get; set; }
        public string ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
        public int[] SubDataAttributes { get; set; }
        private char AttributeType { get; set; }
        public int? TargetProfileId { get; set; }
        public ReportAttributeType ReportAttributeType
        {
            get
            {
                return (ReportAttributeType)AttributeType;
            }
        }
    }

    public class SubReportDto
    {
        public int FeatureId { get; set; }
        public string HasSubReports { get; set; }
    }

    public enum ReportAttributeType
    {
        Simple = 'S',
        Complex = 'C',
        Derived = 'D'
    }

    public enum ReportAttribute
    {
        EcoScore,
        FuelConsumption,
        CruiseCcontrol
    }
}
