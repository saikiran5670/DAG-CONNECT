using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface IGeofenceRepository
    {
        Task<bool> DeleteGeofence(GeofenceDeleteEntity objGeofenceDeleteEntity);
        Task<Geofence> CreatePolygonGeofence(Geofence geofence, bool IsBulkImport = false);
        Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest);
        Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofence, bool IsBulkImport = false);
        Task<Geofence> UpdatePolygonGeofence(Geofence geofence);
        Task<IEnumerable<Geofence>> GetGeofenceByGeofenceID(int organizationId, int geofenceId);
        Task<List<Geofence>> BulkImportGeofence(List<Geofence> geofences);
        Task<Geofence> UpdateCircularGeofence(Geofence geofence);
        Task<IEnumerable<Geofence>> GetAllGeofence(Geofence geofenceFilter);
    }
}
