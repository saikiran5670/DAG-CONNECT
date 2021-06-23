using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public interface IReportCreator
    {
        void SetParameters(ReportCreationScheduler reportSchedulerData);
        Task<byte[]> GenerateReport();
    }
}
