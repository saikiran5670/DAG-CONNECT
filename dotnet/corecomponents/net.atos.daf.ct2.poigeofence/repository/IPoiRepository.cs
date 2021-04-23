using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface IPoiRepository
    {
        Task<List<POIEntityResponse>> GetAllGobalPOI(POIEntityRequest objPOIEntityRequest);
        Task<List<POI>> GetAllPOI(POI poi);
        Task<POI> CreatePOI(POI poi);
        Task<bool> UpdatePOI(POI poi);
        Task<bool> DeletePOI(int poiId);

    }
}