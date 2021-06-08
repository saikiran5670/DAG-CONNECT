using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.CustomValidators.Alert;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class NotificationLimit
    {
        //public int Id { get; set; }
        //public int NotificationId { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Notification mode type should be 1 character")]
        [NotificationModeTypeCheck(ErrorMessage = "Notification mode type is invalid.")]
        public string NotificationModeType { get; set; }
        public int MaxLimit { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Notification period type should be 1 character")]
        [NotificationPeriodTypeCheck(ErrorMessage = "Notification period type is invalid.")]
        public string NotificationPeriodType { get; set; }
        public int PeriodLimit { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
    }
    public class NotificationLimitEdit : NotificationLimit
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        //public string State { get; set; }
        //public long ModifiedAt { get; set; }
    }
}
