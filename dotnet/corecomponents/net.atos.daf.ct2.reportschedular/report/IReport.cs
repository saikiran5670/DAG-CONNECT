using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public interface IReport
    {
        Task SetParameters(ReportCreationScheduler reportSchedulerData);

        Task<string> GenerateSummary();

        Task<string> GenerateTable();

        Task<string> GenerateTemplate(byte[] logoBytes);
    }
}
