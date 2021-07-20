using System;
using Microsoft.Extensions.Caching.Memory;

namespace net.atos.daf.ct2.rfmsdataservice.Common
{
    public class MemoryCacheProvider : IMemoryCacheProvider
    {
        private const int CACHE_SECONDS = 100; // 10 Seconds
        private readonly IMemoryCache _cache;
        public MemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }
        public T GetFromCache<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T cachedResponse);
            return cachedResponse as T;
        }
        public void SetCache<T>(string key, T value) where T : class
        {
            SetCache(key, value, DateTimeOffset.Now.AddSeconds(CACHE_SECONDS));
        }
        public void SetCache<T>(string key, T value, DateTimeOffset duration) where T : class
        {
            _cache.Set(key, value, duration);
        }
        public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class
        {
            _cache.Set(key, value, options);
        }
        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
