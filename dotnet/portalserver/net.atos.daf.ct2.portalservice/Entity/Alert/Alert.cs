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
        [IsEmpty(ErrorMessage = "Name should not be null or empty.")]
        [StringLength(50, MinimumLength = 1,
                  ErrorMessage = "Alert name should be between 1 and 50 characters")]
        public string Name { get; set; }
        [AlertCategory]
        public string Category { get; set; }

        public string Type { get; set; }

        public string ValidityPeriodType { get; set; }

        public long ValidityStartDate { get; set; }

        public long ValidityEndDate { get; set; }

        public int VehicleGroupId { get; set; }
        [State]
        public string State { get; set; }
        public string ApplyOn { get; set; }

        //public long CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        //public long ModifiedAt { get; set; }

        //public int ModifiedBy { get; set; }

    }
    public class Alert:AlertBase
    {
         public int Id { get; set; }
         public int IsDuplicate { get; set; }

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
