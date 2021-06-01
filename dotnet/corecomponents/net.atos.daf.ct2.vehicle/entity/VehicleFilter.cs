namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleFilter
    {
        public int VehicleId { get; set; }
        public int OrganizationId { get; set; }
        public int AccountId { get; set; }
        public int VehicleGroupId { get; set; }
        public int AccountGroupId { get; set; }
        public int FeatureId { get; set; }
        public string VehicleIdList { get; set; }
        public string VIN { get; set; }
        public VehicleStatusType Status { get; set; }




    }
}
