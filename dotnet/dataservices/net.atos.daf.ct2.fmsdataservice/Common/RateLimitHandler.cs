using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.fmsdataservice.entity;
using System.Net;

namespace net.atos.daf.ct2.fmsdataservice.Common
{
    public class RateLimitHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILog _logger;
        private readonly IConfiguration _configuration;
        private IMemoryCacheProvider _cache;

        public RateLimitHandler(RequestDelegate next,
                                IConfiguration configuration)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IMemoryCacheProvider cache)
        {
            _cache = cache;
            await this.ProcessRequestAsync(context).ConfigureAwait(false);
        }
        private async Task ProcessRequestAsync(HttpContext context)
        {
            //Pull Authorized feature for this request fromt he context
            string authorizedfeature = Convert.ToString(context.Items["AuthorizedFeature"]);
            bool hasVin = context.Request.Query.ContainsKey("vin");
            bool hasSince = context.Request.Query.ContainsKey("since");
            var requestParameter = context.Request.Query.ToDictionary(x => x.Key.ToLower(), x => x.Value.ToString());
            //Null Check for authorized feature
            //Necessary check though this is mandatory and will never be null
            //Before it would come to this point if it is null a 403 unauthorized response would have been already sent
            if (!string.IsNullOrEmpty(authorizedfeature))
            {
                string emailAddress = string.Empty;
                //Extract Email Claim from the authenticated user claims 
                var emailClaim = context.User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                //Extract email address claim for rate limiting check 
                if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
                {
                    emailAddress = emailClaim.Value;
                    _logger.Info($"[FMSDataService - Rate Limiter] Email claim received for Rate Limit Management: {emailClaim}");
                    //Get Associated Rate for the given API Feature
                    //var featureRateName = await _rfmsManager.GetRFMSFeatureRate(emailAddress, RateLimitConstants.RATE_LIMIT_FEATURE_NAME);
                    //if (featureRateName != null)
                    {
                        //Fetch Max Rate & Period from Configuration
                        var maxRate = 1;
                        var period = 60;
                        string cacheKeyExtention = string.Empty;
                        if (hasVin)
                        {
                            cacheKeyExtention = requestParameter["vin"];
                        }
                        if (hasSince)
                        {
                            cacheKeyExtention += requestParameter["since"];
                        }
                        string cacheKey = $"{emailAddress}_{authorizedfeature}{cacheKeyExtention}";
                        RateLimitData rateLimitCacheEntry = _cache.GetFromCache<RateLimitData>(cacheKey);
                        if (rateLimitCacheEntry == null)
                        {
                            // Key not in cache, so get data.
                            // Set cache options.
                            //Set cache expiry equal to the time period of the rate limit requests so that the cache will be empty after that time.
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetAbsoluteExpiration(TimeSpan.FromSeconds(Convert.ToInt32(period)));

                            //Fill Rate Limit Object to store in cache
                            RateLimitData rateLimitData = Map(Convert.ToInt32(maxRate), Convert.ToInt32(period), authorizedfeature);

                            // Save data in cache.
                            _cache.SetCache(cacheKey, rateLimitData, cacheEntryOptions);
                            _logger.Info($"[FMSDataService - Rate Limiter] Cache Key saved for Rate Limit Management: {cacheKey}");

                            //Generate Response with expected response headers
                            WriteResponseHeader(context,
                                                rateLimitData.MaxRateLimit,
                                                rateLimitData.RemainingRateCount,
                                                rateLimitData.ResetTime,
                                                cacheKey,
                                                false);
                        }
                        else
                        {
                            if (rateLimitCacheEntry.ElapsedRateCount < rateLimitCacheEntry.MaxRateLimit)
                            {
                                //Increment Elapsed Count
                                rateLimitCacheEntry.ElapsedRateCount++;

                                //Decrement Remaining Count
                                rateLimitCacheEntry.RemainingRateCount--;

                                //Generate Response with expected response headers
                                WriteResponseHeader(context,
                                                    rateLimitCacheEntry.MaxRateLimit,
                                                    rateLimitCacheEntry.RemainingRateCount,
                                                    rateLimitCacheEntry.ResetTime,
                                                    cacheKey,
                                                    false);

                            }
                            else
                            {
                                //Generate Response with expected response headers with code 429
                                WriteResponseHeader(context,
                                                   rateLimitCacheEntry.MaxRateLimit,
                                                   rateLimitCacheEntry.RemainingRateCount,
                                                   rateLimitCacheEntry.ResetTime,
                                                   cacheKey,
                                                   true);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.Info($"[Rate Limit Process request failed - authorized feature is empty] ");
            }
            await _next.Invoke(context);
        }

        private void WriteResponseHeader(HttpContext context, int maxRate, int remainingRate, long resetTime, string cacheKey, bool isLimitExceeded)
        {
            if (!isLimitExceeded)
            {
                context.Response.Headers.Add("x-rate-limit-limit", maxRate.ToString());
                context.Response.Headers.Add("x-rate-limit-remaining", remainingRate.ToString());
                context.Response.Headers.Add("X-Rate-Limit-Reset", resetTime.ToString());

                _logger.Info($"[FMSDataService - Rate Limiter] " +
                                                 $"Cache Key:{cacheKey}, " +
                                                 $"Max Rate: {maxRate}, " +
                                                 $"Remaining Limit: {remainingRate}, " +
                                                 $"Reset: {resetTime}");
            }
            else
            {
                context.Response.StatusCode = 429;
                context.Response.Headers.Add("Retry-After", resetTime.ToString());
                _logger.Info($"[FMSDataService - Rate Limiter] " +
                             $"Cache Key:{cacheKey}, " +
                             $"Max Rate Limit reached, response 429 sent. Retry after: {resetTime}");
            }
        }

        private RateLimitData Map(int maxRate, int period, string authorizedFeature)
        {
            //Calculate the reet time for the request in UTC
            long resetTime = utilities.UTCHandling.GetUTCFromDateTime(DateTime.Now.AddSeconds(period)) / 1000;

            return new RateLimitData
            {
                MaxRateLimit = Convert.ToInt32(maxRate),
                Period = Convert.ToInt32(period),
                RateLimitAPI = authorizedFeature,
                RemainingRateCount = Convert.ToInt32(maxRate) - 1,
                ElapsedRateCount = 1,
                FirstRequestTime = DateTime.Now,
                ResetTime = resetTime
            };
        }
    }
}
