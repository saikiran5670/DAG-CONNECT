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
        Task<Geofence> CreateGeofence(Geofence geofence);
    }
}
