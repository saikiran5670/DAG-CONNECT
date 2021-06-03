namespace net.atos.daf.ct2.reports.entity
{
    public class LiveFleetPosition
    {
        public long GpsAltitude { get; set; }
        public long GpsHeading { get; set; }
        public long GpsLatitude { get; set; }
        public long GpsLongitude { get; set; }
        public int Id { get; set; }
        public string TripId { get; set; }
    }
}
