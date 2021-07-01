using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity.fleetFuel
{
    public class AverageTrafficClassification
    {
        public int Id { get; set; }
        public decimal MaxValue { get; set; }
        public decimal MinValue { get; set; }
        public string Key { get; set; }
    }
}
