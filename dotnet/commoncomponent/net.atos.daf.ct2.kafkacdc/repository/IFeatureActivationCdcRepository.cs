using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IFeatureActivationCdcRepository
    {
        Task<List<VehicleAlertRef>> GetVehiclesAndAlertFromSubscriptionConfiguration(int subscriptionId);
        Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertId);
        Task<IEnumerable<int>> GetAlertFeatureIds();
        Task<List<VehicleAlertRef>> GetAlertandVin(List<int> vehicleIds);
    }
}
