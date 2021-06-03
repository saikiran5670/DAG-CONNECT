namespace net.atos.daf.ct2.identitysession.entity
{
    public class AccountAssertion
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string SessionState { get; set; }
        public string AccountId { get; set; }
        public string CreatedAt { get; set; }
        public int Session_Id { get; set; }
    }
}