namespace net.atos.daf.ct2.vehicle.entity
{
    public class ProvisioningVehicleDataServiceRequest
    {
        public int OrgId { get; set; }
        public string Account { get; set; }
        public string DriverId { get; set; }
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
    }
}
