using System;

namespace net.atos.daf.ct2.authenticationservicerest.Entity
{
   public class AuthToken
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string SessionState { get; set; }
        public string Scope { get; set; }
    }
}
