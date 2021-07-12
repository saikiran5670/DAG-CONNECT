using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class IdlingConsumption
    {
        public int Id { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
