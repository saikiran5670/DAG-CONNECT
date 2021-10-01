using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.httpclient.entity
{
    public class VehiclesStatusOverviewRequest
    {
        public List<string> vin { get; set; }
        public string Language { get; set; }
        public string Retention { get; set; }
    }
}
