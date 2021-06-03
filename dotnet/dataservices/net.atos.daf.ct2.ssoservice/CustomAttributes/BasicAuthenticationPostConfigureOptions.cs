using System;
using Microsoft.Extensions.Options;

namespace net.atos.daf.ct2.singlesignonservice.CustomAttributes
{
    public class BasicAuthenticationPostConfigureOptions : IPostConfigureOptions<BasicAuthenticationOptions>
    {
        public void PostConfigure(string name, BasicAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.ApplicationName))
            {
                throw new InvalidOperationException("ApplicationName must be provided in options");
            }
        }
    }
}
