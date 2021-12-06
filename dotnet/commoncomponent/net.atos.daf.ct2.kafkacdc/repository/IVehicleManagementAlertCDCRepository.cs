using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IVehicleManagementAlertCDCRepository
    {
        Task<List<VehicleAlertRef>> GetVehicleAlertRefFromvehicleId(IEnumerable<int> alertIds, IEnumerable<int> vehicleIds);
        Task<List<VehicleAlertRef>> GetVehicleAlertByvehicleId(IEnumerable<int> vehicleIds, int organizationId);
        Task<List<VehicleGroupAlertRef>> GetAlertByVehicleAndFeatures(List<int> vehicleGroupIds, List<int> featureIds);
        Task<bool> DeleteAndInsertVehicleAlertRef(List<int> alertIds, IEnumerable<int> vehicleIds, List<VehicleAlertRef> vehicleAlertRefs);
    }
}
