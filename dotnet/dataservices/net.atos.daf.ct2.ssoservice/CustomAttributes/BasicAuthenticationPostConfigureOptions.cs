using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
