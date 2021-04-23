using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public class GeofenceManager: IGeofenceManager
    {
        private readonly IGeofenceRepository geofenceRepository;
        public async Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID)
        {
            return await geofenceRepository.DeleteGeofence(geofenceIds, organizationID);
        }
        public async Task<Geofence> CreateGeofence(Geofence geofence)
        {
            return await geofenceRepository.CreateGeofence(geofence);
        }
        public async Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest)
        {
            return await geofenceRepository.GetAllGeofence(geofenceEntityRequest);
        }
    }
}
