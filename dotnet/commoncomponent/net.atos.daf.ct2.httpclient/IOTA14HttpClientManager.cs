using System.Threading.Tasks;
using net.atos.daf.ct2.httpclientfactory.entity.ota22;

namespace net.atos.daf.ct2.httpclientfactory
{
    public interface IOTA14HttpClientManager
    {
        Task<VehiclesStatusOverviewResponse> PostManagerApproval(VehiclesStatusOverviewRequest request);
    }
}