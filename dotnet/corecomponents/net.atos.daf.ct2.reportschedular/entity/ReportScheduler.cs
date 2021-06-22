using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportScheduler
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ReportId { get; set; }
        public string FrequencyType { get; set; }
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
        public string MailSubject { get; set; }
        public string MailDescription { get; set; }
        public long ReportDispatchTime { get; set; }
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
        public string FileName { get; set; }
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
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }
    public class ReportSchedulerMap : ReportScheduler
    {
        public List<ScheduledReport> ScheduledReport { get; set; } = new List<ScheduledReport>();
        public List<ScheduledReportRecipient> ScheduledReportRecipient { get; set; } = new List<ScheduledReportRecipient>();
        public List<ScheduledReportVehicleRef> ScheduledReportVehicleRef { get; set; } = new List<ScheduledReportVehicleRef>();
        public List<ScheduledReportDriverRef> ScheduledReportDriverRef { get; set; } = new List<ScheduledReportDriverRef>();
    }
}
