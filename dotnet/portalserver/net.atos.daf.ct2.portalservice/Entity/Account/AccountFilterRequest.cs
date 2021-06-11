namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountFilterRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Email { get; set; }
        public string AccountIds { get; set; }
        public string Name { get; set; }
        public int AccountGroupId { get; set; }
    }
}
