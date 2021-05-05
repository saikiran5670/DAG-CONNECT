using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
   public interface ICorridorManger
    {
        Task<List<CorridorResponse>> GetCorridorList(CorridorRequest objCorridorRequest);
    }
}
