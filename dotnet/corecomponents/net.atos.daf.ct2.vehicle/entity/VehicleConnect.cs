using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleConnect
    {
        public int VehicleId { get; set; }
        public char Opt_In { get; set; }
        public int ModifiedBy { get; set; }

    }
    public class VehicleConnectResponse
    {
        public List<int> VehicleConnectedList { get; set; }
        public List<int> VehicleNotConnectedList { get; set; }
    }
}
