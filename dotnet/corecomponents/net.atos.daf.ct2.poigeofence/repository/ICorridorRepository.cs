using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ICorridorRepository
    {
        Task<List<CorridorResponse>> GetCorridorList(CorridorRequest objCorridorRequest);

        Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor);
    }
}
