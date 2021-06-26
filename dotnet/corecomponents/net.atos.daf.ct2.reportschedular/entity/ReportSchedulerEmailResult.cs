using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportSchedulerEmailResult
    {
        public int ReportSchedulerId { get; set; }
        public int OrganizationId { get; set; }
        public string FrequencyType { get; set; }
        public long LastScheduleRunDate { get; set; }
        public long NextScheduleRunDate { get; set; }
        public string MailSubject { get; set; }
        public int ReportCreatedBy { get; set; }
        public string MailDescription { get; set; }
        public string ReportToken { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public string EmailId { get; set; }
        public string LanguageCode { get; set; }
        public bool IsMailSent { get; set; }

    }
}
