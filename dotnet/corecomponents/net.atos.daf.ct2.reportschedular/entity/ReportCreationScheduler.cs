using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportCreationScheduler : ReportScheduler
    {
        public string ReportName { get; set; }

        public string ReportKey { get; set; }

        public int TimeZoneId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public int UnitId { get; set; }

        public int VehicleDisplayId { get; set; }

        public int DateFormatId { get; set; }

        public int TimeFormatId { get; set; }
    }
}
