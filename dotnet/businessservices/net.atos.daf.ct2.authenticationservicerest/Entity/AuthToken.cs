using System;

namespace net.atos.daf.ct2.authenticationservicerest.Entity
{
    public class AuthToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }  
        public string token_type { get; set; }
        public string session_state { get; set; }
        public string scope { get; set; }
        public string user_name { get; set; }
        //public string locale{ get; set; }
        //public string timezone{ get; set; }
        //public string unit{ get; set; }
        //public string currency{ get; set; }
        //public string date_format{ get; set; }
    }
}
