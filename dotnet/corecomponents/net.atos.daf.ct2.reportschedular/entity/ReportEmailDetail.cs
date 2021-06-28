using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportEmailDetail
    {
        public int ReportId { get; set; }
        public string EmailId { get; set; }
        public bool IsMailSent { get; set; }

    }
}
