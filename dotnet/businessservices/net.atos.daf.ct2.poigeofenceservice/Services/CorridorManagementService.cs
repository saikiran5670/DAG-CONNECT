using log4net;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.poigeofence;
using System.Reflection;

namespace net.atos.daf.ct2.poigeofenceservice
{
    public class CorridorManagementService: CorridorService.CorridorServiceBase
    {

        private ILog _logger;
        private readonly ICorridorManger _corridorManger;
        public CorridorManagementService(ICorridorManger corridorManger)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorManger = corridorManger;

        }
    }
}
