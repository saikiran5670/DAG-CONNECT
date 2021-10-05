using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace net.atos.daf.ct2.otasoftwareupdate.common
{
    public class CampiagnDataCaching
    {
        private readonly IMemoryCache _cache;

        public CampiagnDataCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<string> GetReleaseNotesFromCache(CampiagnData searchData)
        {
            string result = string.Empty;
            if (_cache.TryGetValue(CacheKeyConstants.CAMPAIGN_DATA_KEY, out FixedSizedQueue cacheCampaignData))
            {
                result = cacheCampaignData.Find(searchData);
            }
            return Task.FromResult(result);
        }
        public Task InsertReleaseNotesToCache(CampiagnData searchData, int limit = 100, int expirayInDays = 100)
        {
            if (_cache.TryGetValue(CacheKeyConstants.CAMPAIGN_DATA_KEY, out FixedSizedQueue cacheCampaignData))
            {
                cacheCampaignData.Enqueue(searchData);
            }
            else
            {
                FixedSizedQueue obj = new FixedSizedQueue { Limit = limit };
                obj.Enqueue(searchData);
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(expirayInDays));
                // Save data in cache.
                _cache.Set(CacheKeyConstants.CAMPAIGN_DATA_KEY, obj, cacheEntryOptions);
            }
            return Task.CompletedTask;
        }
    }
}
