using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.responce;

namespace net.atos.daf.ct2.rfms.repository
{
    public interface IRfmsRepository
    {
        Task<RfmsVehicles> Get(RfmsVehicleRequest rfmsVehicleRequest);

         Task<RfmsVehiclePositionRequest> Get(RfmsVehiclePositionRequest rfmsVehiclePositionRequest);
           
    }
}