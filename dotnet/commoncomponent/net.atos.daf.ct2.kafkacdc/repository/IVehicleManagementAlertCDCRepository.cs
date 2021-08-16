using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IVehicleManagementAlertCDCRepository
    {
        Task<List<VehicleAlertRef>> GetVehicleAlertRefFromvehicleId(int vehicleId);
        Task<List<VehicleAlertRef>> GetVehicleAlertByvehicleId(int vehicleId);
    }
}
