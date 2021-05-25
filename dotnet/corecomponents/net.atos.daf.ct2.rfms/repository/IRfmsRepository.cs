using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.responce;

namespace net.atos.daf.ct2.rfms.repository
{
    public interface IRfmsRepository
    {
        Task<RfmsVehicles> GetVehicles(RfmsVehicleRequest rfmsVehicleRequest);

         Task<RfmsVehiclePositionRequest> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest);
           
    }
}