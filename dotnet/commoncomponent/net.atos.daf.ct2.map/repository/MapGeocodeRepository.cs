
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.map.repository
{
    public class MapGeocodeRepository : IMapGeocodeRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IMapGeocodeManager _mapGeocodeManager;
        private static readonly log4net.ILog log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MapGeocodeRepository()
        {

        }

    }

  
}
