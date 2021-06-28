using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleCountFilter
    {
        public int VehicleGroupId { get; set; }
        public string GroupType { get; set; }
        public string FunctionEnum { get; set; }
        public int OrgnizationId { get; set; }
    }
}
