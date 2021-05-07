using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class RouteCorridorFilter
    {
        public int ID { get; set; }
        public string Search { get; set; }
        public char CorridorType { get; set; }
        public string CorridorLabel { get; set; }
        public string State { get; set; }
        public int? OrganizationId { get; set; }
    }
}
