namespace net.atos.daf.ct2.driver.entity
{
    public class ProvisioningDriverDataServiceRequest
    {
        public string OrgId { get; set; }
        public string VIN { get; set; }
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
    }
}
