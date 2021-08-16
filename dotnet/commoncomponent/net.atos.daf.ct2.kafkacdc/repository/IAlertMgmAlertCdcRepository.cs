using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IAlertMgmAlertCdcRepository
    {
        Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(int alertId);
        Task<List<VehicleAlertRef>> GetVehiclesFromAlertConfiguration(int alertId);
        Task<bool> InsertVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs);
        Task<bool> UpdateVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs);
        Task<bool> DeleteVehicleAlertRef(List<int> alertIds);
        Task<bool> DeleteAndInsertVehicleAlertRef(List<int> alertIds, List<VehicleAlertRef> vehicleAlertRefs);
    }
}
