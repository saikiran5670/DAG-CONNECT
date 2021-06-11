namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleGroupRequest
    {
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleCount { get; set; }
        public int UserCount { get; set; }
        public bool IsGroup { get; set; }
    }
}
