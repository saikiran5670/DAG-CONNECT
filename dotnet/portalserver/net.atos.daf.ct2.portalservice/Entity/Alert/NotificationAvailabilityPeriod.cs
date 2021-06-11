using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class NotificationAvailabilityPeriod
    {
        //public int Id { get; set; }
        //public int NotificationId { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Availability period type should be 1 character")]
        public string AvailabilityPeriodType { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Period type should be 1 character")]
        public string PeriodType { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
    }
    public class NotificationAvailabilityPeriodEdit : NotificationAvailabilityPeriod
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        //public string State { get; set; }
        //public long ModifiedAt { get; set; }
    }
}
