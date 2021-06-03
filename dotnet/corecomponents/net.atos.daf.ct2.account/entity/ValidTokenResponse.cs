namespace net.atos.daf.ct2.account.entity
{
    public class ValidTokenResponse
    {
        public string TokenIdentifier { get; set; }
        public int SessionId { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public bool Valid { get; set; }
    }
}
