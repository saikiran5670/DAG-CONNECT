using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ICorridorRepository
    {
        Task<List<CorridorResponse>> GetCorridorListByOrganization(CorridorRequest objCorridorRequest);
        Task<List<ViaAddressDetail>> GetCorridorViaStopById(int Id);
        Task<CorridorEditViewResponse> GetCorridorListByOrgIdAndCorriId(CorridorRequest objCorridorRequest);
        Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor);
        List<ExistingTrip> GetExistingtripListByCorridorId(int corridoreid);
        List<Nodepoint> GetTripNodes(string tripid, int landmarkid);
        Task<List<CorridorResponse>> GetExistingTripCorridorListByOrganization(CorridorRequest objCorridorRequest);
        Task<ExistingTripCorridor> AddExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
        Task<ExistingTripCorridor> UpdateExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
        Task<CorridorID> DeleteCorridor(int CorridorId);
        Task<int> GetAssociateAlertbyId(int CorridorId);
        Task<bool> CheckRouteCorridorIsexist(string Name, int? OrganizationId, int id, char type);

        Task<RouteCorridor> UpdateRouteCorridor(RouteCorridor routeCorridor);
        Task<bool> CheckCorridorexistByIdName(string CorridorName, int? OrganizationId, int Id, char Type);
    }
}
