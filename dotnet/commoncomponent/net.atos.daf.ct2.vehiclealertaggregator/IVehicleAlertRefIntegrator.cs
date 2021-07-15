using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehiclealertaggregator.entity;

namespace net.atos.daf.ct2.vehiclealertaggregator
{
    public interface IVehicleAlertRefIntegrator
    {
        Task GetVehicleAlertRefFromVehicleManagement(List<int> vins);
        Task GetVehicleAlertRefFromAlertConfiguration(List<int> alertIds);
        Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts);
        Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds);
    }
}
