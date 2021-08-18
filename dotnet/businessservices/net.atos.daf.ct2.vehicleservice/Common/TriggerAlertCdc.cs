﻿using System;
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
        public async Task TriggerAlertCdc(IEnumerable<int> vehicleIds)
            => _ = await Task.Run(() => _vehicleMgmAlertCdcManager.GetVehicleAlertRefFromVehicleId(vehicleIds));
        public async Task TriggerVehicleGroupCdc(int vehicleGroupId)
           => _ = await Task.Run(() => _vehicleGroupAlertCdcManager.GetVehicleGroupAlertConfiguration(vehicleGroupId));
    }
}
