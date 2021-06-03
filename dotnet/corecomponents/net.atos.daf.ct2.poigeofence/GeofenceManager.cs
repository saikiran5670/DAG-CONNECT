using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence
{
    public class GeofenceManager : IGeofenceManager
    {
        private readonly IGeofenceRepository _geofenceRepository;

        public GeofenceManager(IGeofenceRepository geofenceRepository)
        {
            this._geofenceRepository = geofenceRepository;
        }
        public async Task<bool> DeleteGeofence(GeofenceDeleteEntity objGeofenceDeleteEntity)
        {
            return await _geofenceRepository.DeleteGeofence(objGeofenceDeleteEntity);
        }
        public async Task<Geofence> CreatePolygonGeofence(Geofence geofence)
        {
            return await _geofenceRepository.CreatePolygonGeofence(geofence);
        }
        public async Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest)
        {
            return await _geofenceRepository.GetAllGeofence(geofenceEntityRequest);
        }
        public async Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofence)
        {
            return await _geofenceRepository.CreateCircularGeofence(geofence);
        }
        public async Task<Geofence> UpdatePolygonGeofence(Geofence geofence)
        {
            return await _geofenceRepository.UpdatePolygonGeofence(geofence);
        }
        public async Task<IEnumerable<Geofence>> GetGeofenceByGeofenceID(int organizationId, int geofenceId)
        {
            return await _geofenceRepository.GetGeofenceByGeofenceID(organizationId, geofenceId);
        }
        public async Task<List<Geofence>> BulkImportGeofence(List<Geofence> geofences)
        {
            return await _geofenceRepository.BulkImportGeofence(geofences);
        }
        public async Task<Geofence> UpdateCircularGeofence(Geofence geofence)
        {
            return await _geofenceRepository.UpdateCircularGeofence(geofence);
        }
        public async Task<IEnumerable<Geofence>> GetAllGeofence(Geofence geofenceFilter)
        {
            return await _geofenceRepository.GetAllGeofence(geofenceFilter);
        }
    }
}
