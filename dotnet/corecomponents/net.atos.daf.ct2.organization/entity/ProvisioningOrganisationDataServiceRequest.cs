namespace net.atos.daf.ct2.organization.entity
{
    public class ProvisioningOrganisationDataServiceRequest
    {
        public string Account { get; set; }
        public string DriverId { get; set; }
        public string VIN { get; set; }
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
    }
}
