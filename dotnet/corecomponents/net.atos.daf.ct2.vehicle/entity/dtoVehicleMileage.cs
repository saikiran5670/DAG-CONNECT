using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class dtoVehicleMileage
    {
        public int id { get; set; }
        public long evt_timestamp { get; set; }
        public int odo_mileage { get; set; }
        public int odo_distance { get; set; }
        public int real_distance { get; set; }
        public string vin { get; set; }
        public long modified_at { get; set; }
    }
}
