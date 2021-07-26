using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace net.atos.daf.ct2.rfmsdataservice.Common
{
    public static class RateLimitHandlerExtension
    {
        public static IApplicationBuilder UseRateLimitation(this IApplicationBuilder app)
        {
            app.UseMiddleware<RateLimitHandler>();
            return app;
        }
    }
}
