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

        public AlertCdcHelper(IVehicleManagementAlertCDCManager vehicletMgmAlertCdcManager)
        {
            _vehicleMgmAlertCdcManager = vehicletMgmAlertCdcManager;
        }
        public async Task TriggerAlertCdc(IEnumerable<int> vehicleIds)
            => _ = await Task.Run(() => _vehicleMgmAlertCdcManager.GetVehicleAlertRefFromVehicleId(vehicleIds));
    }
}
