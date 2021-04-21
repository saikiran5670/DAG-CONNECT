using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofenceservice;

namespace net.atos.daf.ct2.poigeofenceservice
{
    public class PoiGeofenceManagementService:PoiGeofenceService.PoiGeofenceServiceBase
    {
        private ILog _logger;
        private readonly IPoiManager _poiManager ;
        public PoiGeofenceManagementService(IPoiManager poiManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _poiManager = poiManager;
        }
    }
}
