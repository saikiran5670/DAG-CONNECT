using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public class CorridorManger : ICorridorManger
    {
        private readonly ICorridorRepository _corridorRepository;
        public CorridorManger(ICorridorRepository corridorRepository)
        {
            _corridorRepository = corridorRepository;
        }

        public async Task<ExistingTripCorridor> AddExistingTripCorridor(ExistingTripCorridor existingTripCorridor)
        {
            return await _corridorRepository.AddExistingTripCorridor(existingTripCorridor);
        }

        public async Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor)
        {
            return await _corridorRepository.AddRouteCorridor(routeCorridor);
        }

        //public async Task<List<CorridorResponse>> GetCorridorList(CorridorRequest objCorridorRequest)
        //{
        //    return await _corridorRepository.GetCorridorList(objCorridorRequest);
        //}

        public async Task<CorridorLookUp> GetCorridorList(CorridorRequest objCorridorRequest)
        {
            CorridorLookUp objCorridorLookUp = new CorridorLookUp();

            if (objCorridorRequest.OrganizationId > 0 && objCorridorRequest.CorridorId > 0)
            {
                objCorridorLookUp.EditView = await _corridorRepository.GetCorridorListByOrgIdAndCorriId(objCorridorRequest);
                    //loop to get existing trip corridore details.
                    foreach (var item in objCorridorLookUp.EditView)
                    {
                        item.ViaAddressDetails = await _corridorRepository.GetCorridorViaStopById(item.Id);
                        if ((LandmarkType)item.CorridorType.ToCharArray()[0] == LandmarkType.ExistingTripCorridor)
                        {
                            item.CorridoreTrips = _corridorRepository.GetExistingtripListByCorridorId(objCorridorRequest.CorridorId);
                            foreach (var trips in item.CorridoreTrips)
                            {
                             trips.NodePoints = _corridorRepository.GetTripNodes(trips.TripId);
                            }
                        }
                    }
                    
                
            }

            if (objCorridorRequest.OrganizationId > 0)
            {
                objCorridorLookUp.GridView = await _corridorRepository.GetCorridorListByOrganization(objCorridorRequest);
                for (int i = 0; i < objCorridorLookUp.GridView.Count; i++)
                {
                    objCorridorLookUp.GridView[i].ViaAddressDetails = await _corridorRepository.GetCorridorViaStopById(objCorridorLookUp.GridView[i].Id);
                }
                //get existing trip corridore
                var existingtripcoridor = await _corridorRepository.GetExistingTripCorridorListByOrganization(objCorridorRequest);
                foreach (var item in existingtripcoridor)
                {
                    if ((LandmarkType)item.CorridorType.ToCharArray()[0] == LandmarkType.ExistingTripCorridor)
                    {
                        item.CorridoreTrips = _corridorRepository.GetExistingtripListByCorridorId(objCorridorRequest.CorridorId);
                        foreach (var trips in item.CorridoreTrips)
                        {
                            trips.NodePoints = _corridorRepository.GetTripNodes(trips.TripId);
                        }
                    }
                }
            }
            
            return objCorridorLookUp;
        }

        #region GetExitingTrip


        #endregion
    }
}
