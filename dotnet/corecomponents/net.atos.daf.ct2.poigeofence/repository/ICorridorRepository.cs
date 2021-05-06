using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ICorridorRepository
    {
        Task<List<CorridorResponse>> GetCorridorListByOrganization(CorridorRequest objCorridorRequest);
        Task<List<ViaAddressDetail>> GetCorridorViaStopById(int Id);
        Task<List<CorridorEditViewResponse>> GetCorridorListByOrgIdAndCorriId(CorridorRequest objCorridorRequest);
        Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor);
        Task<ExistingTripCorridor> AddExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
    }
}
