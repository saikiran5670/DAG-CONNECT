using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface ILandmarkAlertCdcManager
    {
        //Task GetVehicleAlertRefFromVehicleManagement(List<int> vins);
        Task<bool> LandmarkAlertRefFromAlertConfiguration(int landmarkId, string operation, string landmarktype);
        //Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts);
        //Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds);
    }
}
