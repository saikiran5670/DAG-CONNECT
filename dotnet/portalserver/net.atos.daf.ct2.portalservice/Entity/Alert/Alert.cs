using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using net.atos.daf.ct2.portalservice.CustomValidators.Alert;
using net.atos.daf.ct2.portalservice.CustomValidators.Common;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertBase
    {
        //public int Id { get; set; }
        public int OrganizationId { get; set; }
        [Required(ErrorMessage = "Name should not be null or empty.")]
        [StringLength(50, MinimumLength = 1,ErrorMessage = "Alert name should be between 1 and 50 characters")]
        [RegularExpression(@"^[a-zA-ZÀ-ÚÄ-Ü0-9]([\w -]*[a-zA-ZÀ-ÚÄ-Ü0-9])?$", ErrorMessage = "Only alphabets,numbers,hyphens,dash,spaces,periods,international alphabets allowed in alert name.")]
        public string Name { get; set; }
        [AlertCategory]
        [StringLength(1, MinimumLength = 1,ErrorMessage = "Alert Category should be 1 character")]
        public string Category { get; set; }
        [StringLength(1, MinimumLength = 1,ErrorMessage = "Alert Type should be 1 character")]
        public string Type { get; set; }
        [StringLength(1, MinimumLength = 1,ErrorMessage = "Alert Validity Period Type should be 1 character")]
        public string ValidityPeriodType { get; set; }

        public long ValidityStartDate { get; set; }

        public long ValidityEndDate { get; set; }

        public int VehicleGroupId { get; set; }
        [State]
        [StringLength(1, MinimumLength = 1,ErrorMessage = "Alert State should be 1 character")]
        public string State { get; set; }
        [StringLength(1, MinimumLength = 1,ErrorMessage = "ApplyOn should be 1 character")]
        public string ApplyOn { get; set; }

        //public long CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        //public long ModifiedAt { get; set; }

        //public int ModifiedBy { get; set; }

    }
    public class Alert:AlertBase
    {
         public int Id { get; set; }
         public bool IsDuplicate { get; set; }

        //public long CreatedAt { get; set; }

        //public long ModifiedAt { get; set; }

        //public int ModifiedBy { get; set; }

        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public List<AlertUrgencyLevelRef> AlertUrgencyLevelRefs { get; set; } = new List<AlertUrgencyLevelRef>();

        public List<AlertLandmarkRef> AlertLandmarkRefs { get; set; } = new List<AlertLandmarkRef>();
    }
    public class AlertEdit: AlertBase
    {
        [Required(ErrorMessage = "Alert id is Required")]
        public int Id { get; set; }
        //public int OrganizationId { get; set; }
        //public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public List<NotificationEdit> Notifications { get; set; } = new List<NotificationEdit>();
        public List<AlertUrgencyLevelRefEdit> AlertUrgencyLevelRefs { get; set; } = new List<AlertUrgencyLevelRefEdit>();
        public List<AlertLandmarkRefEdit> AlertLandmarkRefs { get; set; } = new List<AlertLandmarkRefEdit>();
    }
}
