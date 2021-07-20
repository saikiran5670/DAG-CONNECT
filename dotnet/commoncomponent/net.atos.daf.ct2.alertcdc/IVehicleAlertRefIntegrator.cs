using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.alertcdc.entity;

namespace net.atos.daf.ct2.alertcdc
{
    public interface IVehicleAlertRefIntegrator
    {
        Task GetVehicleAlertRefFromVehicleManagement(List<int> vins);
        Task GetVehicleAlertRefFromAlertConfiguration(List<int> alertIds);
        Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts);
        Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds);
    }
}
