using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public interface IGeofenceManager
    {
        Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID);
        Task<Geofence> CreatePolygonGeofence(Geofence geofence);
        Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest);
        Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofence);
        Task<Geofence> UpdatePolygonGeofence(Geofence geofence);
    }
}
