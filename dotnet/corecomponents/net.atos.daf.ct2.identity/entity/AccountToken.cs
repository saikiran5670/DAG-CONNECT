using System.Net;

namespace net.atos.daf.ct2.identity.entity
{
    public class AccountToken
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string SessionState { get; set; }
        public string Scope { get; set; }
        public HttpStatusCode statusCode { get; set; }
        public string message { get; set; }
        public int RoleID { get; set; }
        public int OrganizationID { get; set; }
        // public string Error { get; set; }
    }
}