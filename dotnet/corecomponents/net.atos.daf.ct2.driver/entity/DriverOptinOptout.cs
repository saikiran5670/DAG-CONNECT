namespace net.atos.daf.ct2.driver.entity
{
    public class DriverOptinOptout
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int DriverId { get; set; }
        public string Status { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }
}
