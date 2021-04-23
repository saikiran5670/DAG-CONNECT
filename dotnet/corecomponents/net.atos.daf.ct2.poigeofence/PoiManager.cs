using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public class PoiManager : IPoiManager
    {
        private readonly IPoiRepository _poiRepository;
        public PoiManager(IPoiRepository poiRepository)
        {
            _poiRepository = poiRepository;
        }

        public async Task<List<POIEntityResponse>> GetAllGobalPOI(POIEntityRequest objPOIEntityRequest)
        {
            return await _poiRepository.GetAllGobalPOI(objPOIEntityRequest);
        }
        public async Task<List<POI>> GetAllPOI(POI poi)
        {
            return await _poiRepository.GetAllPOI(poi);
        }
        public async Task<POI> CreatePOI(POI poi)
        {
            return await _poiRepository.CreatePOI(poi);
        }
        public async Task<bool> UpdatePOI(POI poi)
        {
            return await _poiRepository.UpdatePOI(poi);
        }
        public async Task<bool> DeletePOI(int poiId)
        {
            return await _poiRepository.DeletePOI(poiId);
        }
    }
}
