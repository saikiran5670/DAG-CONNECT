using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.responce;

namespace net.atos.daf.ct2.rfms
{
    public interface IRfmsManager
    {
        Task<RfmsVehicles> Get(RfmsVehicleRequest rfmsVehicleRequest);

        Task<RfmsVehiclePositionRequest> Get(RfmsVehiclePositionRequest rfmsVehiclePositionRequest);
           

        
    }
}