using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class VehicleConnectionSettings
    {
        public int VehicleId { get; set; }
        public char Opt_In { get; set; }
        public int ModifiedBy { get; set; }
    }
}
