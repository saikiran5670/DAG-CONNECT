using Microsoft.AspNetCore.Authentication;

namespace net.atos.daf.ct2.customerdataservice.CustomAttributes
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApplicationName { get; set; }
    }
}
