using Microsoft.AspNetCore.Authentication;

namespace net.atos.daf.ct2.provisioningdataservice.CustomAttributes
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApplicationName { get; set; }
    }
}
