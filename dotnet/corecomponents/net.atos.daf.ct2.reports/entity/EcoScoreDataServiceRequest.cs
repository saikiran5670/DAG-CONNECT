namespace net.atos.daf.ct2.reports.entity
{
    public enum AggregateType
    {
        TRIP = -1,
        DAY = 0,
        WEEK = 6,
        MONTH = 29
    }

    public class EcoScoreDataServiceRequest
    {
        public string AccountEmail { get; set; }
        public string DriverId { get; set; }
        public string OrganizationId { get; set; }
        public string VIN { get; set; }
        public AggregateType AggregationType { get; set; }
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }
        public int MinDistance { get; set; }
    }
}
