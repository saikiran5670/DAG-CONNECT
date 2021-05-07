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

        public async Task<CorridorLookUp> GetCorridorList(CorridorRequest objCorridorRequest)
        {
            CorridorLookUp objCorridorLookUp = new CorridorLookUp();

            if (objCorridorRequest.OrganizationId > 0 && objCorridorRequest.CorridorId > 0)
            {
                objCorridorLookUp.EditView = await _corridorRepository.GetCorridorListByOrgIdAndCorriId(objCorridorRequest);
                //loop to get existing trip corridore details.
                for (int i = 0; i < objCorridorLookUp.EditView.Count; i++)
                {
                    objCorridorLookUp.EditView[i].ViaAddressDetails = await _corridorRepository.GetCorridorViaStopById(objCorridorLookUp.EditView[i].Id);
                }
            }
            else if (objCorridorRequest.OrganizationId > 0 && objCorridorRequest.CorridorId <= 0)
            {
                objCorridorLookUp.GridView = await _corridorRepository.GetCorridorListByOrganization(objCorridorRequest);
                for (int i = 0; i < objCorridorLookUp.GridView.Count; i++)
                {
                    objCorridorLookUp.GridView[i].ViaAddressDetails = await _corridorRepository.GetCorridorViaStopById(objCorridorLookUp.GridView[i].Id);
                }
            }
            return objCorridorLookUp;
        }

        #region GetExitingTrip


        #endregion
    }
}
