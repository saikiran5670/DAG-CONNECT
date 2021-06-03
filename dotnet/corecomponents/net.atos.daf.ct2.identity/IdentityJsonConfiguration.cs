namespace net.atos.daf.ct2.identity
{
    public class IdentityJsonConfiguration
    {
        public string Realm { get; set; }
        public string BaseUrl { get; set; }
        public string AuthUrl { get; set; }
        public string UserMgmUrl { get; set; }
        public string AuthClientId { get; set; }
        public string AuthClientSecret { get; set; }
        public string UserMgmClientId { get; set; }
        public string UserMgmClientSecret { get; set; }
        // public string ReferralUrl { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        //public string ReferralId { get; set; }
        public string RsaPrivateKey { get; set; }
        public string RsaPublicKey { get; set; }

    }
}