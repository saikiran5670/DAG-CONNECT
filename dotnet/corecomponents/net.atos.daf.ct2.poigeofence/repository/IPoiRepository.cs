using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface IPoiRepository
    {
        Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest);
        Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID);
        Task<Geofence> CreateGeofence(Geofence geofence);

        Task<List<POI>> GetAllPOI(POI poi);
        Task<POI> CreatePOI(POI poi);
        Task<bool> UpdatePOI(POI poi);
        Task<bool> DeletePOI(int poiId);

    }
}