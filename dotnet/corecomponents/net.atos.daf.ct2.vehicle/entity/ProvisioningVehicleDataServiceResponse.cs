using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class ProvisioningVehicleDataServiceResponse
    {
        public List<ProvisioningVehicle> Vehicles { get; set; }
    }

    public class ProvisioningVehicle
    {
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }
        public string VIN { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
    }

}
