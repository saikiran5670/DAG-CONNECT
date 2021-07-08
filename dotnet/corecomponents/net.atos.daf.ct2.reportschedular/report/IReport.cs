using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public interface IReport
    {
        void SetParameters(ReportCreationScheduler reportSchedulerData, IEnumerable<VehicleList> vehicleLists);

        Task<string> GenerateSummary();

        Task<string> GenerateTable();

        Task<string> GenerateTemplate(byte[] logoBytes);
    }
}
