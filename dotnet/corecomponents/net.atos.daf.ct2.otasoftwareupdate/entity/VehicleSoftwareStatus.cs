using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.otasoftwareupdate.entity
{
    public class VehicleSoftwareStatus
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Enum { get; set; }
        public string ParentEnum { get; set; }
        public string Key { get; set; }
        public int FeatureId { get; set; }
    }
}
