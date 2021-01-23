using System;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.vehicleservicerest.Entity
{
    public class VehicleFilterRequest
    {
        public int VehicleId { get; set; }
        public int OrganizationId { get; set; }
        public string VehicleIdList { get; set; }
        public string VIN { get; set; }
        public VehicleStatusType Status { get; set; }
    }
}
