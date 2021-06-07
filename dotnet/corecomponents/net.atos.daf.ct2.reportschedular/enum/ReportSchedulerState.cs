using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.ENUM
{
    public enum ReportSchedulerState
    {
        None = 0,
        Active = 'A',
        Suspend = 'I',
        Delete = 'D'
    }
}
