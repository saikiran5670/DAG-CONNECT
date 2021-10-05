using System.Threading.Tasks;
using net.atos.daf.ct2.httpclientfactory.entity.ota22;

namespace net.atos.daf.ct2.httpclientfactory
{
    public interface IOTA22HttpClientManager
    {
        Task<VehiclesStatusOverviewResponse> GetVehiclesStatusOverview(VehiclesStatusOverviewRequest request);
        Task<VehicleUpdateDetailsResponse> GetVehicleUpdateDetails(VehicleUpdateDetailsRequest request);
        Task<CampiagnSoftwareReleaseNoteResponse> GetSoftwareReleaseNote(CampiagnSoftwareReleaseNoteRequest request);
    }
}