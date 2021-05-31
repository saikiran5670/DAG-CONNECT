using System;
using net.atos.daf.ct2.map.entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.map.repository;

namespace net.atos.daf.ct2.map
{
    public class MapManager : IMapManager
    {
        private readonly IMapRepository _mapRepository;
        public MapManager(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }
        public async Task<bool> AddLookupAddress(List<LookupAddress> lookupAddresses)
        {
           return await _mapRepository.AddLookupAddress(lookupAddresses);
        }

        public async Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses)
        {
            return await _mapRepository.GetLookupAddress(lookupAddresses);
        }
    }
}
