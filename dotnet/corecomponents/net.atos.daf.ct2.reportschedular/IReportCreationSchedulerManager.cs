using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reportscheduler
{
    public interface IReportCreationSchedulerManager
    {
        Task<bool> GenerateReport();
    }
}
