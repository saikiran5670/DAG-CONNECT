using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.ENUM;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.poigeofenceservice.common
{
    public class LandmarkAlertCdcHelper
    {
        private readonly ILandmarkAlertCdcManager _landmarkAlertCdcManager;

        public LandmarkAlertCdcHelper(ILandmarkAlertCdcManager landmarkAlertCdcManager)
        {
            _landmarkAlertCdcManager = landmarkAlertCdcManager;
        }
        public async Task TriggerAlertCdc(int landmarkid, string landmarkType)
        {
            await Task.Run(() => _landmarkAlertCdcManager.LandmarkAlertRefFromAlertConfiguration(landmarkid, "U", landmarkType));
        }
    }
}
