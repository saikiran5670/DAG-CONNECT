using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public interface IPoiManager
    {
        Task<List<POI>> GetAllGobalPOI(POIEntityRequest objPOIEntityRequest);
        Task<List<POI>> GetAllPOI(POI poi);
        Task<POI> CreatePOI(POI poi);
        Task<POI> UpdatePOI(POI poi);
        Task<bool> DeletePOI(int poiId);
        Task<bool> DeletePOI(List<int> poiIds);
        Task<UploadPOIExcel> UploadPOI(UploadPOIExcel uploadPOIExcel);
        Task<List<TripEntityResponce>> GetAllTripDetails(TripEntityRequest tripEntityRequest);
    }
}