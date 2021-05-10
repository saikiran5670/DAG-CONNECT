using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
   public interface ICorridorManger
    {
        Task<CorridorLookUp> GetCorridorList(CorridorRequest objCorridorRequest);
        Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor);
        Task<ExistingTripCorridor> AddExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
        Task<ExistingTripCorridor> UpdateExistingTripCorridor(ExistingTripCorridor existingTripCorridor);
        Task<CorridorID> DeleteCorridor(int CorridorId);
        Task<RouteCorridor> UpdateRouteCorridor(RouteCorridor routeCorridor);
    }
}
