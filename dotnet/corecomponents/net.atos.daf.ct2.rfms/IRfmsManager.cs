using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.rfms
{
    public interface IRfmsManager
    {
        Task<RfmsVehicles> GetVehicles(string lastVin, int threshold, int accountId, int orgId);
        Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest, List<VisibilityVehicle> visibleVehicles);
        Task<string> GetRFMSFeatureRate(string emailId, string featureName);
        Task<RfmsVehicleStatus> GetRfmsVehicleStatus(RfmsVehicleStatusRequest rfmsVehicleStatusRequest, List<VisibilityVehicle> visibleVehicles);
    }
}