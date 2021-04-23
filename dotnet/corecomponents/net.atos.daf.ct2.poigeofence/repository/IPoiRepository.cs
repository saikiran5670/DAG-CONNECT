using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface IPoiRepository
    {
        Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest);
        Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID);
    }
}