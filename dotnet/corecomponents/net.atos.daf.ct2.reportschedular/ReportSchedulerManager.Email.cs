using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {
        public async Task<int> SendReportEmail()
        {
            return await _reportSchedulerRepository.SendReportEmail();
        }
    }
}
