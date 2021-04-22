using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence
{
    public class PoiManager : IPoiManager
    {
        private readonly IPoiRepository _poiRepository;
        public PoiManager(IPoiRepository poiRepository)
        {
            _poiRepository = poiRepository;
        }
    }
}
