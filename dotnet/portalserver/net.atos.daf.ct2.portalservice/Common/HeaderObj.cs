namespace net.atos.daf.ct2.portalservice.Common
{
    public class HeaderObj
    {
        public int RoleId { get; set; }
        public int AccountId { get; set; }
        public int OrgId { get; set; }
        public int ContextOrgId => OrgId;
    }
}
