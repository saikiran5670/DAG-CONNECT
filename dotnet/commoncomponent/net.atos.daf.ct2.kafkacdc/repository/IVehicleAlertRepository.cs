using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IVehicleAlertRepository
    {
        Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertIds);
        Task<List<VehicleAlertRef>> GetVehiclesFromAlertConfiguration(List<int> alertIds);
        Task<bool> InsertVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs);
        Task<bool> UpdateVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs);
        Task<bool> DeleteVehicleAlertRef(List<int> alertIds);
        Task<bool> DeleteAndInsertVehicleAlertRef(List<int> alertIds, List<VehicleAlertRef> vehicleAlertRefs);
    }
}
