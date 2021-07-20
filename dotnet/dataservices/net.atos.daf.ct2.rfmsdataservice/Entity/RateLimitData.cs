using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class RateLimitData
    {
        public string RateLimitAPI { get; set; }
        public int MaxRateLimit { get; set; }

        public int Period { get; set; }

        public int ElapsedRateCount { get; set; }

        public int RemainingRateCount { get; set; }

        public DateTime FirstRequestTime { get; set; }

        public long ResetTime { get; set; }
    }

    public class RateLimitConstants
    {
        public const string RATE_LIMIT_FEATURE_NAME = "api.rfms3#rate";

        public const string RATE_LIMIT_CONFIGURATION_MAX_RATE = "MaxRate";

        public const string RATE_LIMIT_CONFIGURATION_PERIOD = "Period";
    }
}
