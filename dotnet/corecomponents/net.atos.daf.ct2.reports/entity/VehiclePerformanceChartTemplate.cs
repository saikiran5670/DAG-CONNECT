using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehiclePerformanceChartTemplate
    {
        public VehiclePerformanceSummary VehiclePerformanceSummary { get; set; }
        public List<VehChartTemplate> VehChartList { get; set; }
    }
    public class VehChartTemplate
    {
        public string XAxisValue { get; set; }
        public string YYxisValue { get; set; }
        public int KpiType { get; set; }
        public string EngineName { get; set; }
        public string ModelName { get; set; }
        public string VehiclePerformanceType { get; set; }
        public int VehPerToYIndex { get; set; }
        public int EngineTypeId { get; set; }
        public string AxisType { get; set; }
        public string AxisValue { get; set; }
    }
}
