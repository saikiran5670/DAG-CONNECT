using System;
using System.Collections.Generic;
using System.Text;
using net.atos.daf.ct2.email.Enum;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportTemplate
    {
        public int ReportId { get; set; }

        public string ReportTranslatedContent { get; set; }

        public string LanguageCode { get; set; }

        public EmailEventType EventType { get; set; }
    }
}
