using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class VehiclePerformanceRequest
    {
        public int Id { get; set; }
        public string Vin { get; set; }
        public string Vid { get; set; }
        public string Engine_Type { get; set; }
        public string Model_Type { get; set; }

    }
}
