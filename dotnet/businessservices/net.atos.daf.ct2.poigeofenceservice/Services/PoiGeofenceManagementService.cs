using System.Reflection;
using log4net;
using net.atos.daf.ct2.poigeofence;

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
