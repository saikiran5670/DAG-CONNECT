using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class LogbookDetailsFilter
    {
        public List<string> GroupId { get; set; }
        public List<string> VIN { get; set; }
        public List<string> AlertLevel { get; set; }
        public List<string> AlertCategory { get; set; }
        public List<string> AlertType { get; set; }
        public long Start_Time { get; set; }
        public long End_time { get; set; }


    }
}
