using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
//using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Text;
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
        public async Task<List<CorridorResponse>> GetCorridorList(CorridorRequest objCorridorRequest)
        {
            return await _corridorRepository.GetCorridorList(objCorridorRequest);
        }
    }
}
