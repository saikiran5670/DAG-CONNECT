namespace net.atos.daf.ct2.email
{
    public class EmailConfiguration
    {
        public string PortalUIBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public bool IsReplyAllowed { get; set; }
        public string ReplyToAddress { get; set; }
    }
}
