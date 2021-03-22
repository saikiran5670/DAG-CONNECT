﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class MemoryCacheProvider: IMemoryCacheProvider
    {
        private const int CacheSeconds = 100; // 10 Seconds
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
            SetCache(key, value, DateTimeOffset.Now.AddSeconds(CacheSeconds));
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
