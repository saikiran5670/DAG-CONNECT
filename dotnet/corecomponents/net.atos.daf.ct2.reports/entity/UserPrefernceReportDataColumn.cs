using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class UserPrefernceReportDataColumn
    {
        public int DataAtrributeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public bool IsExclusive { get; set; }
    }
}
