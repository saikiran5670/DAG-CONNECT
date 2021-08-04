using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.alertservice.common
{
    public class AlertCdcHelper
    {
        private readonly IAlertMgmAlertCdcManager _alertMgmAlertCdcManager;

        public AlertCdcHelper(IAlertMgmAlertCdcManager alertMgmAlertCdcManager)
        {
            _alertMgmAlertCdcManager = alertMgmAlertCdcManager;
        }
        public async Task TriggerAlertCdc(int alertid, string alertState)
        {
            _ = await Task.Run(() => _alertMgmAlertCdcManager.GetVehicleAlertRefFromAlertConfiguration(alertid, alertState));
        }
    }
}
