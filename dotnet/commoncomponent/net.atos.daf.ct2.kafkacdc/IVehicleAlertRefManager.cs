using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IVehicleAlertRefManager
    {
        Task GetVehicleAlertRefFromVehicleManagement(List<int> vins);
        Task<bool> GetVehicleAlertRefFromAlertConfiguration(int alertId, string operation);
        Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts);
        Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds);
    }
}
