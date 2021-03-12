
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Common
{
    public interface IMemoryCacheExtensions
    {
        void setCache<T>(T values, string key);
        T getCache<T>(string key) where T : class;
        void removeCache(string key);

        Task setCacheAsync<T>(T values, string key);
        Task<T> getCacheAsync<T>(string key) where T : class;
        Task removeCacheAsync(string key);
    }
}
