namespace net.atos.daf.ct2.driver.entity
{
    public class DriverOptinOptout
    {
        public int Id { get; set; }
        public int Organization_id { get; set; }
        public int Driver_id { get; set; }
        public string Status { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
    }
}
