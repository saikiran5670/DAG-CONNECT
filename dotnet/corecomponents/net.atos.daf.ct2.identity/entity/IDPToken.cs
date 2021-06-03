namespace net.atos.daf.ct2.identity.entity
{
    public class IDPToken
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public int Refresh_expires_in { get; set; }
        public string Refresh_token { get; set; }
        public string Token_type { get; set; }
        public int Not_before_policy { get; set; }
        public string Session_state { get; set; }
        public string Scope { get; set; }
        public string Error { get; set; }
    }
}