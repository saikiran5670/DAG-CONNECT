using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IVehicleGroupAlertCdcRepository
    {
        Task<List<VehicleAlertRef>> GetVehicleGroupAlertRefByAlertIds(List<int> alertId);
        Task<List<VehicleAlertRef>> GetVehiclesGroupFromAlertConfiguration(int vehicleGroupId, int organizationId);       
        Task<List<AlertGroupId>> GetAlertIdsandVGIds(IEnumerable<int> groupIds, List<int> featureIds);

    }
}
