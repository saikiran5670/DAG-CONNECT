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
        Task<Geofence> CreateGeofence(Geofence geofence);
        Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest);

    }
}
