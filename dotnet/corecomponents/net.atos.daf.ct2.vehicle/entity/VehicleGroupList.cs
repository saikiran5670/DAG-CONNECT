using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleGroupList
    {
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleId { get; set; }        
        public string VehicleName { get; set; }
        public string Vin { get; set; }
        public bool SubcriptionStatus { get; set; }
    }
}
