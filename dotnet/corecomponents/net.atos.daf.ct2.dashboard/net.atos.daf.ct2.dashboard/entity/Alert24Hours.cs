using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.entity
{
    public class Alert24Hours
    {
        public int Logistic { get; set; }
        public int FuelAndDriver { get; set; }
        public int RepairAndMaintenance { get; set; }
        public int Critical { get; set; }
        public int Warning { get; set; }
        public int Advisory { get; set; }

    }
}
