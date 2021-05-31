namespace net.atos.daf.ct2.reports.entity
{
    public class TripFilterRequest
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public string VIN { get; set; }
    }
}
