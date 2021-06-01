namespace net.atos.daf.ct2.alert.entity
{
    public class NotificationLimit
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string NotificationModeType { get; set; }
        public int MaxLimit { get; set; }
        public string NotificationPeriodType { get; set; }
        public int PeriodLimit { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }
}
