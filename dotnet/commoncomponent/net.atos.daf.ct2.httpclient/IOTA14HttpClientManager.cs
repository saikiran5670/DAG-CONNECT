using System.Threading.Tasks;
using net.atos.daf.ct2.httpclientfactory.entity.ota14;

namespace net.atos.daf.ct2.httpclientfactory
{
    public interface IOTA14HttpClientManager
    {
        Task<ScheduleSoftwareUpdateResponse> PostManagerApproval(ScheduleSoftwareUpdateRequest request);
    }
}