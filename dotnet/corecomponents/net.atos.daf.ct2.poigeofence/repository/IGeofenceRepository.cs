using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface IGeofenceRepository
    {
        Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID);
        Task<Geofence> CreatePolygonGeofence(Geofence geofence);
        Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest);
        Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofence);
        Task<Geofence> UpdatePolygonGeofence(Geofence geofence);
        Task<IEnumerable<Geofence>> GetGeofenceByGeofenceID(int organizationId, int geofenceId);
    }
}
