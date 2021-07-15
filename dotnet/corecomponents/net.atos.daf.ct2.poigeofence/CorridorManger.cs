using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

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

            //var _existingTripCorridor = new ExistingTripCorridor();
            //var isExist = _corridorRepository.CheckRouteCorridorIsexist(existingTripCorridor.CorridorLabel, existingTripCorridor.OrganizationId, existingTripCorridor.Id,
            //                                                           Convert.ToChar(existingTripCorridor.CorridorType));
            //if (!await isExist)
            //{
            //     _existingTripCorridor = await _corridorRepository.AddExistingTripCorridor(existingTripCorridor);
            //    //if (corridorID.Id > 0)
            //    //    _existingTripCorridor.Id = corridorID.Id;
            //}
            //else
            //    _existingTripCorridor.Id = -1;
            //return _existingTripCorridor;

            return await _corridorRepository.AddExistingTripCorridor(existingTripCorridor);
        }

        public async Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor)
        {
            RouteCorridor routeCorridor1 = new RouteCorridor();
            var isExist = _corridorRepository.CheckRouteCorridorIsexist(routeCorridor.CorridorLabel, routeCorridor.OrganizationId, routeCorridor.Id, routeCorridor.CorridorType);
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

        public async Task<CorridorID> DeleteCorridor(int corridorId)
        {
            CorridorID corridorID = new CorridorID();
            var isAlertExist = await _corridorRepository.GetAssociateAlertbyId(corridorId);

            if (isAlertExist <= 0)
            {
                var deleteID = await _corridorRepository.DeleteCorridor(corridorId);
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
                if (objCorridorLookUp.EditView != null)
                {
                    objCorridorLookUp.EditView.ViaAddressDetails = await _corridorRepository.GetCorridorViaStopById(objCorridorLookUp.EditView.Id);
                    if ((LandmarkType)objCorridorLookUp.EditView.CorridorType.ToCharArray()[0] == LandmarkType.ExistingTripCorridor)
                    {
                        objCorridorLookUp.EditView.CorridoreTrips = _corridorRepository.GetExistingtripListByCorridorId(objCorridorRequest.CorridorId, out string vIN);
                        objCorridorLookUp.EditView.VIN = vIN;
                        //This is to bind latest End Lat and long by sequence Number from nodes table
                        NodeEndLatLongResponse objNodeEndLatLongResponse = await _corridorRepository.GetExistingTripCorridorListByLandMarkId(objCorridorLookUp.EditView.Id);
                        if (objNodeEndLatLongResponse != null)
                        {
                            objCorridorLookUp.EditView.EndLat = objNodeEndLatLongResponse.EndLat;
                            objCorridorLookUp.EditView.EndLong = objNodeEndLatLongResponse.EndLong;
                            objCorridorLookUp.EditView.EndPoint = objNodeEndLatLongResponse.Address;
                        }

                        foreach (var trips in objCorridorLookUp.EditView.CorridoreTrips)
                        {
                            trips.NodePoints = _corridorRepository.GetTripNodes(trips.TripId, objCorridorLookUp.EditView.Id);
                        }
                    }
                }
                else
                {
                    return objCorridorLookUp;
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
                        //This is to bind latest End Lat and long by sequence Number from nodes table
                        NodeEndLatLongResponse objNodeEndLatLongResponse = await _corridorRepository.GetExistingTripCorridorListByLandMarkId(item.Id);
                        if (objNodeEndLatLongResponse != null)
                        {
                            item.EndLat = objNodeEndLatLongResponse.EndLat;
                            item.EndLong = objNodeEndLatLongResponse.EndLong;
                            item.EndPoint = objNodeEndLatLongResponse.Address;
                        }

                        item.CorridoreTrips = _corridorRepository.GetExistingtripListByCorridorId(item.Id, out string vin);
                        foreach (var trips in item.CorridoreTrips)
                        {
                            trips.NodePoints = _corridorRepository.GetTripNodes(trips.TripId, item.Id);
                        }
                    }
                }
                objCorridorLookUp.GridView.AddRange(existingtripcoridor);
            }
            return objCorridorLookUp;
        }

        public async Task<ExistingTripCorridor> UpdateExistingTripCorridor(ExistingTripCorridor existingTripCorridor)
        {

            var varExistingTripCorridor = new ExistingTripCorridor();
            var isExist = await _corridorRepository.CheckRouteCorridorIsexist(existingTripCorridor.CorridorLabel, existingTripCorridor.OrganizationId, existingTripCorridor.Id,
                                                                       Convert.ToChar(existingTripCorridor.CorridorType));
            //wrong condition removed
            if (!isExist)
            {
                varExistingTripCorridor = await _corridorRepository.UpdateExistingTripCorridor(existingTripCorridor);
            }
            else
                varExistingTripCorridor.Id = -1;
            return varExistingTripCorridor;


        }

        public async Task<RouteCorridor> UpdateRouteCorridor(RouteCorridor objRouteCorridor)
        {
            RouteCorridor objRouteCorridorResponse = new RouteCorridor();
            try
            {
                var isExist = await _corridorRepository.CheckCorridorexistByIdName(objRouteCorridor.CorridorLabel, objRouteCorridor.OrganizationId, objRouteCorridor.Id, objRouteCorridor.CorridorType);
                if (isExist)
                {
                    var corridorID = await _corridorRepository.UpdateRouteCorridor(objRouteCorridor);
                    if (corridorID.Id > 0)
                        objRouteCorridorResponse.Id = corridorID.Id;
                }
                else
                    objRouteCorridorResponse.Id = -1;
            }
            catch (Exception)
            {
                throw;
            }
            return objRouteCorridorResponse;
        }

        #region GetExitingTrip


        #endregion
    }
}
