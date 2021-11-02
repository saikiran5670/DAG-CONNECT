using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.kafkacdc.entity
{
    public class VehicleAlertRef
    {
        public string VIN { get; set; }
        public int AlertId { get; set; }
        public string Op { get; set; }
    }

    public class AlertFromPackage
    {
        public int Alertid { get; set; }
        public int Vehicle_group_id { get; set; }
        public int Featureid { get; set; }
        public int Organizationid { get; set; }
    }
}
