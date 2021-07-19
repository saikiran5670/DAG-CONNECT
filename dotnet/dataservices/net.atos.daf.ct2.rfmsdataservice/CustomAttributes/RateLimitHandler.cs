using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.rfms;
using Microsoft.Extensions.Configuration;

namespace net.atos.daf.ct2.rfmsdataservice.CustomAttributes
{
    public class RateLimitHandler
    {
        private readonly RequestDelegate _next;
        private readonly IRfmsManager _rfmsManager;
        private readonly ILog _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public RateLimitHandler(RequestDelegate next,
                                IRfmsManager rfmsManager,
                                IMemoryCache memoryCache,
                                IConfiguration configuration)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _rfmsManager = rfmsManager;
            _next = next;
            _cache = memoryCache;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            await this.ProcessRequestAsync(context).ConfigureAwait(false);
        }
        private async Task ProcessRequestAsync(HttpContext context)
        {
            //Enable Logging wherever possible
            /*****************************************************************************
             * CLEAN THE CODE AND REFACTOR THE COMPLEX CODE IN NEW METHODS & CLASSES
             * **************************************************************************/
            string authorizedfeature = Convert.ToString(context.Items["AuthorizedFeature"]);
            if (!string.IsNullOrEmpty(authorizedfeature))
            {
                string emailAddress = string.Empty;
                var emailClaim = context.User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                //Extract Session Id for rate limiting check 
                if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
                {
                    emailAddress = emailClaim.Value;
                    _logger.Info($"[rFMSDataService] Email claim received for Rate Limit Management: {emailClaim}");
                    //Get Associated Rate for the given API Feature
                    //Add Null Check
                    var featureRateName = await _rfmsManager.GetRFMSFeatureRate(emailAddress, RateLimitConstants.RATE_LIMIT_FEATURE_NAME);
                    //Add Null Check
                    string configRateName = featureRateName.ToString().Split('#')[1];
                    //Fetch Max Rate & Period from Configuration
                    var maxRate = _configuration.GetSection(configRateName).GetSection(RateLimitConstants.RATE_LIMIT_CONFIGURATION_MAX_RATE).Value;
                    var period = _configuration.GetSection(configRateName).GetSection(RateLimitConstants.RATE_LIMIT_CONFIGURATION_PERIOD).Value;

                    string cacheKey = emailAddress + "_" + authorizedfeature;
                    // Look for cache key.
                    if (!_cache.TryGetValue(cacheKey, out RateLimitData rateLimitCacheEntry))
                    {
                        // Key not in cache, so get data.

                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(Convert.ToInt32(period)));

                        RateLimitData rateLimitData = new RateLimitData();
                        rateLimitData.MaxRateLimit = Convert.ToInt32(maxRate);
                        rateLimitData.Period = Convert.ToInt32(period);
                        rateLimitData.RateLimitAPI = authorizedfeature;
                        rateLimitData.RemainingRateCount = Convert.ToInt32(maxRate) - 1;
                        rateLimitData.ElapsedRateCount = 1;
                        rateLimitData.FirstRequestTime = DateTime.Now;

                        // Save data in cache.
                        _cache.Set(cacheKey, rateLimitData, cacheEntryOptions);

                        context.Response.Headers.Add("x-rate-limit-limit", rateLimitData.MaxRateLimit.ToString());
                        context.Response.Headers.Add("x-rate-limit-remaining", rateLimitData.RemainingRateCount.ToString());
                        context.Response.Headers.Add("X-Rate-Limit-Reset", rateLimitData.Period.ToString());
                    }
                    else
                    {
                        string resetTime = Math.Round((rateLimitCacheEntry.Period - DateTime.Now.Subtract(rateLimitCacheEntry.FirstRequestTime).TotalSeconds), 0).ToString();
                        if (rateLimitCacheEntry.ElapsedRateCount < rateLimitCacheEntry.MaxRateLimit)
                        {
                            rateLimitCacheEntry.ElapsedRateCount++;
                            rateLimitCacheEntry.RemainingRateCount--;
                            context.Response.Headers.Add("x-rate-limit-limit", rateLimitCacheEntry.MaxRateLimit.ToString());
                            context.Response.Headers.Add("x-rate-limit-remaining", rateLimitCacheEntry.RemainingRateCount.ToString());
                            context.Response.Headers.Add("X-Rate-Limit-Reset", resetTime);
                        }
                        else
                        {
                            context.Response.StatusCode = 429;
                            context.Response.Headers.Add("Retry-After", resetTime);
                            return;
                        }
                    }
                }
            }
            await _next.Invoke(context);
        }
    }

    //private void SetRateLimitResponseHeaders(int MaxRateLimit)
    //{
    //}
}
