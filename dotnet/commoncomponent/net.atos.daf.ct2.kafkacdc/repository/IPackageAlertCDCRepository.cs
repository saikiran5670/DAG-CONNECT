using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IPackageAlertCdcRepository
    {
        Task<List<VehicleAlertRef>> GetVehiclesAndAlertFromPackageConfiguration(int packageId);
        Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertId);
        Task<List<AlertGroupId>> GetAlertIdsandVGIds(List<int> groupIds, List<int> featureIds);
        Task<IEnumerable<int>> GetAlertPackageIds(int orgContextId, int packageId, List<int> featureIds);
    }
}
