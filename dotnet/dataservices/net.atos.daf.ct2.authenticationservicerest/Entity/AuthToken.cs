using System;

namespace net.atos.daf.ct2.authenticationservicerest.Entity
{
   public class AuthToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}
