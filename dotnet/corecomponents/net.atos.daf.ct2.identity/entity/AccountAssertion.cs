namespace net.atos.daf.ct2.identity.entity
{
    public class AccountAssertion
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string SessionState { get; set; }
        public string AccountId { get; set; }
        public string CreatedAt { get; set; }
    }
}