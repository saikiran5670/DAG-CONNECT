using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms.repository
{
    public interface IRfmsRepository
    {
        Task<RfmsVehicles> GetVehicles(string visibleVins, int lastVinId);

        Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest, string visibleVins, int lastVinId);

        Task<string> GetRFMSFeatureRate(string emailId, string featureName);

        Task<List<MasterTableCacheObject>> GetMasterTableCacheData();

        Task<RfmsVehicleStatus> GetRfmsVehicleStatus(RfmsVehicleStatusRequest rfmsVehicleStatusRequest, string visibleVins, int lastVinId);
    }
}