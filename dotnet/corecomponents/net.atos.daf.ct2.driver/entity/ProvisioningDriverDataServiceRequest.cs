namespace net.atos.daf.ct2.driver.entity
{
    public class ProvisioningDriverDataServiceRequest
    {
        public int OrgId { get; set; }
        // For current driver
        public string VIN { get; set; }
        // For driver list
        public string[] VINs { get; set; }
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
    }
}
