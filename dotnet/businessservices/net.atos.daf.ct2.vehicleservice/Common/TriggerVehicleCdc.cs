using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.vehicleservice.Common
{
    public class VehicleCdcHelper
    {
        private readonly IVehicleGroupAlertCdcManager _vehicleGroupAlertCdsManager;
        public VehicleCdcHelper(IVehicleGroupAlertCdcManager vehicleGroupAlertCdcManager)
        {
            _vehicleGroupAlertCdsManager = vehicleGroupAlertCdcManager;
        }

    }
}
