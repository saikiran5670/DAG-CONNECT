using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportschedular.entity
{
    public class ReportSchedular
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ReportId { get; set; }
        public int VehicleGroupId { get; set; }
        public int DriverId { get; set; }
        public char FrequencyType { get; set; }
        public char Status { get; set; }
        public char Type { get; set; }
        public string FileName { get; set; }
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
    }
    public class ScheduledReportRecipient
    {
        public int Id { get; set; }      
        public int ScheduleReportId { get; set; }
        public string Email { get; set; }
        public char State { get; set; }      
        public long CreatedAt { get; set; }       
        public long ModifiedAt { get; set; }
    }
}
