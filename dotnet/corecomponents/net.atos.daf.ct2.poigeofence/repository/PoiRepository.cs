using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public class PoiRepository : IPoiRepository
    {
        private readonly IDataAccess _dataAccess;
        public PoiRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
    }
}
