using System.Threading.Tasks;
using net.atos.daf.ct2.httpclient.entity;
using net.atos.daf.ct2.httpclient.ENUM;

namespace net.atos.daf.ct2.httpclient
{
    public interface IOTA22HttpClientManager
    {
        Task<VehiclesStatusOverviewResponse> GetVehiclesStatusOverview(VehiclesStatusOverviewRequest request);
    }
}