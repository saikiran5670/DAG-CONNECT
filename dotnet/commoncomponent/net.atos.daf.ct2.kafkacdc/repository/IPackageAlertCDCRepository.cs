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
    }
}
