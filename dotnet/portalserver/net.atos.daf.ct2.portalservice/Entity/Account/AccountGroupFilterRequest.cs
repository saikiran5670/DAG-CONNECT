namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountGroupFilterRequest
    {
        public int AccountGroupId { get; set; }
        public int OrganizationId { get; set; }
        public int AccountId { get; set; }
        public bool Accounts { get; set; }

        public bool AccountCount { get; set; }

    }
}
