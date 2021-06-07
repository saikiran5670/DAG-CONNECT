using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence
{
    public class PoiManager : IPoiManager
    {
        private readonly IPoiRepository _poiRepository;
        public PoiManager(IPoiRepository poiRepository)
        {
            _poiRepository = poiRepository;
        }

        public async Task<List<POI>> GetAllGobalPOI(POIEntityRequest objPOIEntityRequest)
        {
            return await _poiRepository.GetAllGobalPOI(objPOIEntityRequest);
        }
        public async Task<List<POI>> GetAllPOI(POI poi)
        {
            return await _poiRepository.GetAllPOI(poi);
        }
        public async Task<POI> CreatePOI(POI poi)
        {
            return await _poiRepository.CreatePOI(poi);
        }
        public async Task<POI> UpdatePOI(POI poi)
        {
            return await _poiRepository.UpdatePOI(poi);
        }
        public async Task<bool> DeletePOI(int poiId)
        {
            return await _poiRepository.DeletePOI(poiId);
        }

        public async Task<bool> DeletePOI(List<int> poiIds)
        {
            return await _poiRepository.DeletePOI(poiIds);
        }

        public async Task<UploadPOIExcel> UploadPOI(UploadPOIExcel uploadPOIExcel)
        {
            return await _poiRepository.UploadPOI(uploadPOIExcel);
        }

        public async Task<List<TripEntityResponce>> GetAllTripDetails(TripEntityRequest tripEntityRequest)
        {
            return await _poiRepository.GetAllTripDetails(tripEntityRequest);
        }

        public async Task<TripAddressDetails> UpdateTripArddress(TripAddressDetails tripAddressDetails) {

            return await _poiRepository.UpdateTripArddress(tripAddressDetails);
        }
    }
}
