using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.CustomValidators.Common;

namespace net.atos.daf.ct2.portalservice.Entity.ReportScheduler
{
    public class ReportScheduler
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ReportId { get; set; }
        public string FrequencyType { get; set; }
        [State]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Report scheduler Status should be 1 character")]
        public string Status { get; set; }
        public string Type { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public string Code { get; set; }
        public long LastScheduleRunDate { get; set; }
        public long NextScheduleRunDate { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        [Required(ErrorMessage = "Mail Subject should not be null or empty.")]
        [StringLength(120, MinimumLength = 1, ErrorMessage = "Mail subject should be between 1 and 120 characters")]
        [RegularExpression(@"^[a-zA-ZÀ-ÚÄ-Ü0-9]([\w -]*[a-zA-ZÀ-ÚÄ-Ü0-9])?$", ErrorMessage = "Only alphabets,numbers,hyphens,dash,spaces,periods,international alphabets allowed in mail subject.")]
        public string MailSubject { get; set; }
        [Required(ErrorMessage = "Mail Description should not be null or empty.")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Mail description should be between 1 and 250 characters")]
        [RegularExpression(@"^[a-zA-ZÀ-ÚÄ-Ü0-9]([\w -]*[a-zA-ZÀ-ÚÄ-Ü0-9])?$", ErrorMessage = "Only alphabets,numbers,hyphens,dash,spaces,periods,international alphabets allowed in mail description.")]
        public string MailDescription { get; set; }
        public long ReportDispatchTime { get; set; }
        public List<ScheduledReport> ScheduledReport { get; set; } = new List<ScheduledReport>();
        [MinLength(1, ErrorMessage = "At least 1 recipient required.")]
        public List<ScheduledReportRecipient> ScheduledReportRecipient { get; set; } = new List<ScheduledReportRecipient>();
        [MinLength(1, ErrorMessage = "At least 1 vehicle or vehicle group data required.")]
        public List<ScheduledReportVehicleRef> ScheduledReportVehicleRef { get; set; } = new List<ScheduledReportVehicleRef>();
        [MinLength(1, ErrorMessage = "At least 1 driver detail required.")]
        public List<ScheduledReportDriverRef> ScheduledReportDriverRef { get; set; } = new List<ScheduledReportDriverRef>();
    }
    public class ScheduledReport
    {
        public int Id { get; set; }
        public int ScheduleReportId { get; set; }
        public byte Report { get; set; }
        public string Token { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public long DownloadedAt { get; set; }
        public long ValidTill { get; set; }
        public long CreatedAt { get; set; }
    }
    public class ScheduledReportRecipient
    {
        public int Id { get; set; }
        public int ScheduleReportId { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }

    public class ScheduledReportDriverRef
    {
        public int ScheduleReportId { get; set; }
        public int DriverId { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class ScheduledReportVehicleRef
    {
        public int ScheduleReportId { get; set; }
        public int VehicleGroupId { get; set; }
        public int VehicleId { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }
    public class ReportStatusEnableDisableModel
    {
        [Required]
        public int ReportId { get; set; }
        [StateForEnableDisable]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Report scheduler Status should be 1 character Either A or I")]
        public string Status { get; set; }
    }
    public class ReportStatusByIdModel
    {
        [Required]
        public int ReportId { get; set; }
    }
    public class ReportStatusByTokenModel
    {
        [Required]
        public string Token { get; set; }
    }
}
