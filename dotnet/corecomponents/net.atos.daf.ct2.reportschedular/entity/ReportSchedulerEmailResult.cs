using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportSchedulerEmailResult
    {
        public int Repsch_id { get; set; }
        public int Repsch_organization_id { get; set; }
        public string Repsch_frequency_type { get; set; }
        public long Repsch_last_schedule_run_date { get; set; }
        public long Repsch_next_schedule_run_date { get; set; }
        public string Repsch_mail_subject { get; set; }
        public int Repsch_created_by { get; set; }
        public string Repsch_mail_description { get; set; }
        public string Schrep_Token { get; set; }
        public long Repsch_start_date { get; set; }
        public long Repsch_end_date { get; set; }
        public string Receipt_email { get; set; }
        public bool IsMailSent { get; set; }

    }
}
