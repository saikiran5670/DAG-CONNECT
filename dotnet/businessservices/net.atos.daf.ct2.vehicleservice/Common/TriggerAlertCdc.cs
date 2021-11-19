using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.vehicleservice.common
{
    public class AlertCdcHelper
    {
        private readonly IVehicleManagementAlertCDCManager _vehicleMgmAlertCdcManager;
        private readonly IVehicleGroupAlertCdcManager _vehicleGroupAlertCdcManager;
        public AlertCdcHelper(IVehicleManagementAlertCDCManager vehicletMgmAlertCdcManager, IVehicleGroupAlertCdcManager vehicleGroupAlertCdcManager)
        {
            _vehicleMgmAlertCdcManager = vehicletMgmAlertCdcManager;
            _vehicleGroupAlertCdcManager = vehicleGroupAlertCdcManager;
        }
        public async Task TriggerAlertCdc(IEnumerable<int> vehicleIds, string operation, int contextOrgId, int userOrgId, int accountId, IEnumerable<int> featureIds)
            => _ = await Task.Run(() => _vehicleMgmAlertCdcManager.GetVehicleAlertRefFromVehicleId(vehicleIds, operation, contextOrgId, userOrgId, accountId, featureIds));

        public async Task TriggerVehicleGroupCdc(int vehicleGroupId, string alertState, int organizationId, int accountId, int loggedInOrgId, int[] featureIds)
        {
            _ = await Task.Run(() => _vehicleGroupAlertCdcManager.GetVehicleGroupAlertConfiguration(vehicleGroupId, alertState, organizationId, accountId, loggedInOrgId, featureIds));
        }

    }
}
