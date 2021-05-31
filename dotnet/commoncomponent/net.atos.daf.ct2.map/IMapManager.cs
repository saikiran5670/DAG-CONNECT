using net.atos.daf.ct2.map.entity;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace net.atos.daf.ct2.map
{
    public interface IMapManager
    {
        Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses);
        Task<bool> AddLookupAddress(List<LookupAddress> lookupAddresses);
    }
}