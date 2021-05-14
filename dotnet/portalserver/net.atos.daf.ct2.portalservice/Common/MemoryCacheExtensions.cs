using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Common
{

    public class MemoryCacheExtensions: IMemoryCacheExtensions
    {
        public IDistributedCache _cache;
        public BinaryFormatter binaryFormatter;

        public MemoryCacheExtensions(IDistributedCache _cache)
        {
            this.binaryFormatter = new BinaryFormatter();
            this._cache = _cache;
        }

        public async Task<T> getCacheAsync<T>(string key) where T : class
        {
            try
            {
                byte[] values =await _cache.GetAsync(key);

                if (values == null) return null;

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(values))
                {
                    return binaryFormatter.Deserialize(memoryStream) as T;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task setCacheAsync<T>(T values, string key)
        {
            try
            {
                MemoryStream mStream = new MemoryStream();

                binaryFormatter.Serialize(mStream, values);

                mStream.ToArray();

                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(7),
                    SlidingExpiration = TimeSpan.FromSeconds(30)
                };

               await _cache.SetAsync(key, mStream.ToArray());

                mStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task removeCacheAsync(string key)
        {
            try
            {
               await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T getCache<T>(string key) where T : class
        {
            try
            {
                byte[] values = _cache.Get(key);

                if (values == null) return null;

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(values))
                {
                    return binaryFormatter.Deserialize(memoryStream) as T;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setCache<T>(T values, string key)
        {
            try
            {
                MemoryStream mStream = new MemoryStream();

                binaryFormatter.Serialize(mStream, values);

                mStream.ToArray();

                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(7),
                    SlidingExpiration = TimeSpan.FromSeconds(30)
                };

                _cache.Set(key, mStream.ToArray());

                mStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void removeCache(string key)
        {
            try
            {
               _cache.Remove(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
