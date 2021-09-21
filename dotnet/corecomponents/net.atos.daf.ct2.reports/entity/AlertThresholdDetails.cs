using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class AlertThresholdDetails
    {
        public int AlertId { get; set; }
        public string AlertLevel { get; set; }
        public double ThresholdValue { get; set; }
        public string ThresholdUnit { get; set; }

    }
}
