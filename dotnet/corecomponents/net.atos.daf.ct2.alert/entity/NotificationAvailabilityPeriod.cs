namespace net.atos.daf.ct2.alert.entity
{
    public class NotificationAvailabilityPeriod
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string AvailabilityPeriodType { get; set; }
        public string PeriodType { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }
}
