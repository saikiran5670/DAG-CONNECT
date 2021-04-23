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

        public async Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest)
        {
            return await _poiRepository.GetAllPOI(objPOIEntityRequest);
        }
        public async Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID)
        {
            return await _poiRepository.DeleteGeofence(geofenceIds, organizationID);
        }

        public async Task<Geofence> CreateGeofence(Geofence geofence)
        {
            return await _poiRepository.CreateGeofence(geofence);
        }
    }
}
