using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IAlertMgmAlertCdcManager
    {
        //Task GetVehicleAlertRefFromVehicleManagement(List<int> vins);
        Task<bool> GetVehicleAlertRefFromAlertConfiguration(int alertId, string operation);
        Task<bool> GetVehicleAlertRefFromAlertConfiguration(int alertId, string operation, int accountId, int organisationId, int contextOrgId, IEnumerable<int> featureIds);
        //Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts);
        //Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds);
    }
}
