using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class VehicleRequest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string License_Plate_Number { get; set; }
      
    }

    public class VehicleCreateRequest
    {
        public int ID { get; set; }
        public int? Organization_Id { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }
        public string License_Plate_Number { get; set; }
        public string Status { get; set; }

    }
}
