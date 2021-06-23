using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.CustomValidators.Alert;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class NotificationBase
    {
        //public int Id { get; set; }
        //public int AlertId { get; set; }
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Alert urgency level type should be 1 character")]

        //[UrgencyLevelCheck(ErrorMessage = "Urgency level type is invalid.")]
        public string AlertUrgencyLevelType { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Frequency type should be 1 character")]
        [FrequencyTypeCheck(ErrorMessage = "Frequence type is invalid.")]
        public string FrequencyType { get; set; }
        [NotificationFreqThreshholdValueCheck("FrequencyType", ErrorMessage = "Threshhold value required for number of occurrence frequency type.")]
        public int FrequencyThreshholdValue { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Validity type should be 1 character")]
        [ValidityTypeCheck(ErrorMessage = "Validity type is invalid.")]

        public string ValidityType { get; set; } = "A";
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        //public long ModifiedAt { get; set; }
        //public int ModifiedBy { get; set; }
    }
    public class Notification : NotificationBase
    {
        //public int Id { get; set; }
        //public int AlertId { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
        //public int ModifiedBy { get; set; }
        [MaxLength(10, ErrorMessage = "Maximum 10 recipient can be added in notification.")]
        public List<NotificationRecipient> NotificationRecipients { get; set; }
        //[MaxLength(4, ErrorMessage = "Maximum 4 custom period user can add per day.")]
        public List<AlertTimingDetail> AlertTimingDetails { get; set; }
    }
    public class NotificationEdit : NotificationBase
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        [MaxLength(10, ErrorMessage = "Maximum 10 recipient can be added in notification.")]
        public List<NotificationRecipientEdit> NotificationRecipients { get; set; }
        //[MaxLength(4, ErrorMessage = "Maximum 4 custom period user can add per day.")]
        public List<AlertTimingDetailEdit> AlertTimingDetails { get; set; }
    }
}
