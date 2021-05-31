using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.map.entity;

namespace net.atos.daf.ct2.map.repository
{
    public interface IMapRepository
    {
        Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses);
        Task<List<LookupAddress>> AddLookupAddress(List<LookupAddress> lookupAddresses);
    }
}