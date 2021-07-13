using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ICorridorRepository
    {
        Task<List<CorridorResponse>> GetCorridorListByOrganization(CorridorRequest objCorridorRequest);
        Task<List<ViaAddressDetail>> GetCorridorViaStopById(int id);
        Task<CorridorEditViewResponse> GetCorridorListByOrgIdAndCorriId(CorridorRequest objCorridorRequest);
        Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor);
        List<ExistingTrip> GetExistingtripListByCorridorId(int corridoreid);
        List<Nodepoint> GetTripNodes(string tripid, int landmarkid);
        Task<List<CorridorResponse>> GetExistingTripCorridorListByOrganization(CorridorRequest objCorridorRequest);
        Task<ExistingTripCorridor> AddExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
        Task<ExistingTripCorridor> UpdateExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
        Task<CorridorID> DeleteCorridor(int corridorId);
        Task<int> GetAssociateAlertbyId(int corridorId);
        Task<bool> CheckRouteCorridorIsexist(string name, int? organizationId, int id, char type);

        Task<RouteCorridor> UpdateRouteCorridor(RouteCorridor routeCorridor);
        Task<bool> CheckCorridorexistByIdName(string corridorName, int? organizationId, int id, char type);
        Task<NodeEndLatLongResponse> GetExistingTripCorridorListByLandMarkId(int landMarkId);
    }
}
