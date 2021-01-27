using System;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.vehicleservicerest.Entity
{
    public class VehicleCreateRequest
    {
        public int ID { get; set; }
        public int ? Organization_Id { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }       
        public string License_Plate_Number { get; set; }
        public VehicleStatusType Status { get; set; }
    }
}
