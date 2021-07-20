using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alertcdc.entity
{
    public class VehicleAlertRef
    {
        public string VIN { get; set; }
        public int AlertId { get; set; }
        public string State { get; set; }
    }
}
