using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportEmailDetail
    {
        public RecipientDetail RecipientDetail { get; set; }
        public ReportEmailFrequency ReportEmailFrequency { get; set; }

    }
}
