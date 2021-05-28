using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.responce;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms
{
    public interface IRfmsManager
    {
        Task<RfmsVehicles> GetVehicles(RfmsVehicleRequest rfmsVehicleRequest);

        Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest);
           

        
    }
}