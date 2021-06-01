using System.Net;

namespace net.atos.daf.ct2.account.entity
{
    public class SSOToken
    {
        public string Token { get; set; }
        public string TokenType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
    public class TokenSSORequest
    {
        public string Email { get; set; }
        public int AccountID { get; set; }
        public int RoleID { get; set; }
        public int OrganizaitonID { get; set; }
    }
}
