
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Common
{
    public interface IMemoryCacheExtensions
    {
        void SetCache<T>(T values, string key);
        T GetCache<T>(string key) where T : class;
        void RemoveCache(string key);

        Task SetCacheAsync<T>(T values, string key);
        Task<T> GetCacheAsync<T>(string key) where T : class;
        Task RemoveCacheAsync(string key);
    }
}
