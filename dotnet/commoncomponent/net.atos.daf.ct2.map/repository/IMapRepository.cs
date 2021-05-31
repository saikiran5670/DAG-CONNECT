using net.atos.daf.ct2.map.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.map.repository
{
    public interface IMapRepository
    {
        Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses);
        Task<List<LookupAddress>> AddLookupAddress(List<LookupAddress> lookupAddresses);
    }
}