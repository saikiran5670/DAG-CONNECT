using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class KpiDataRange
    {
        public string PerformanceType { get; set; }
        public int Index { get; set; }
        public string Kpi { get; set; }
        public int LowerVal { get; set; }
        public int UpperVal { get; set; }
        public int Value { get; set; }
    }
}
