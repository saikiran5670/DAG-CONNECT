using net.atos.daf.ct2.identitysession.ENUM;

namespace net.atos.daf.ct2.singlesignonservice.Entity
{
    public class AccountToken
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public int ExpireIn { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public TokenType TokenType { get; set; }
        public IDPType IdpType { get; set; }
        public string SessionState { get; set; }
        public long CreatedAt { get; set; }
        public string Scope { get; set; }
        public string Error { get; set; }
        public string TokenId { get; set; }
        public int Session_Id { get; set; }

    }
}
