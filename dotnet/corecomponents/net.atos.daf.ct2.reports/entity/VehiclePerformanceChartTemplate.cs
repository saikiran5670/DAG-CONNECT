using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehiclePerformanceChartTemplate
    {
        public VehiclePerformanceSummary VehiclePerformanceSummary { get; set; }
        public List<EngineLoadType> VehChartList { get; set; }
    }
    public class EngineLoadType
    {
        public string Enginetype { get; set; }
        public string TemplateLableKey { get; set; }
        public bool IsDefault { get; set; }
        public List<Chartdata> Chartdata { get; set; }
    }
    public class Chartdata
    {
        public int Index { get; set; }
        public string Range { get; set; }
        public List<string> Axisvalues { get; set; }
    }
}
