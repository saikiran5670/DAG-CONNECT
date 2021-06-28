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
}
