using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.rfms.responce;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsManager : IRfmsManager
    {
        IRfmsRepository _rfmsRepository;

        public RfmsManager(IRfmsRepository rfmsRepository)
        {
            _rfmsRepository = rfmsRepository;
        }

        public async Task<RfmsVehicles> GetVehicles(RfmsVehicleRequest rfmsVehicleRequest)
        {
            return await _rfmsRepository.GetVehicles(rfmsVehicleRequest);
        }

        public async Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {
            return await _rfmsRepository.GetVehiclePosition(rfmsVehiclePositionRequest);
        }
    }
}
