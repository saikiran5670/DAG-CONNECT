using System.Net;

namespace net.atos.daf.ct2.account.entity
{
    public class SSOToken
    {
        public string token { get; set; }
        public string tokenType { get; set; }
        public HttpStatusCode statusCode { get; set; }
        public string message { get; set; }
    }
    public class TokenSSORequest
    {
        public string Email { get; set; }
        public int AccountID { get; set; }
        public int RoleID { get; set; }
        public int OrganizaitonID { get; set; }
    }
}
