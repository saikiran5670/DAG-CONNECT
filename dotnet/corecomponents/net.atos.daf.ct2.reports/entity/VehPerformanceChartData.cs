using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehPerformanceChartData
    {
        public string Vin { get; set; }
        public string TripId { get; set; }
        public int AbsRpmtTrque { get; set; }
        public int OrdRpmTorque { get; set; }
        public string MatrixValue { get; set; }
        public string CountPerIndex { get; set; }
        public string ColumnIndex { get; set; }
        public List<KPIs> ListKPIs { get; set; }
        public long TripDuration { get; set; }

    }
    public class KPIs
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

}
