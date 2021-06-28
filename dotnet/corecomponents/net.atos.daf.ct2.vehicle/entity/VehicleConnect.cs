using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleConnect
    {
        public int VehicleId { get; set; }
        public char Opt_In { get; set; }
        public int ModifiedBy { get; set; }

    }
    public class VehicleConnectedResult
    {
        public List<VehicleConnect> VehicleConnectedList { get; set; }
        public List<VehicleConnect> VehicleConnectionfailedList { get; set; }
    }
}
