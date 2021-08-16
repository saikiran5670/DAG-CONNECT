using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehiclePerformanceRequest
    {
        public string Vin { get; set; }
        public string PerformanceType { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }

    }

    public class VehiclePerformanceSummary
    {
        public string VehicleName { get; set; }
        public string Vin { get; set; }
        public string EngineType { get; set; }
        public string ModelType { get; set; }
    }

    public class VehPerformanceProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

}
