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
            RouteCorridor routeCorridor1 = new RouteCorridor();
            var isExist = _corridorRepository.CheckRouteCorridorIsexist(routeCorridor.CorridorLabel, routeCorridor.OrganizationId, routeCorridor.Id);
            if (!await isExist)
            {
                var corridorID = await _corridorRepository.AddRouteCorridor(routeCorridor);
                if (corridorID.Id > 0)
                    routeCorridor1.Id = corridorID.Id;
            }
            else
                routeCorridor1.Id = -1;
            return routeCorridor1;
        }

        public async Task<CorridorID> DeleteCorridor(int CorridorId)
        {
            CorridorID corridorID = new CorridorID();
            var isAlertExist = await _corridorRepository.GetAssociateAlertbyId(CorridorId);

            if (isAlertExist <= 0)
            {
                var deleteID = await _corridorRepository.DeleteCorridor(CorridorId);
                if (deleteID.Id > 0)
                    corridorID.Id = deleteID.Id;
                else
                    corridorID.Id = -2;
            }
            else
            {
                corridorID.Id = -1;
            }
            return corridorID;

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

           else if (objCorridorRequest.OrganizationId > 0 && objCorridorRequest.CorridorId <= 0)
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
                        item.CorridoreTrips = _corridorRepository.GetExistingtripListByCorridorId(item.Id);
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
