namespace net.atos.daf.ct2.authenticationservicerest.Entity
{
    public class AuthToken
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public string Token_type { get; set; }
    }
}
