using System.Text;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class HeaderObj
    {
        public int RoleId { get; set; }
        public int AccountId { get; set; }
        public int OrgId { get; set; }
        public int ContextOrgId { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{nameof(AccountId)} : {AccountId}\n");
            sb.Append($"{nameof(RoleId)} : {RoleId}\n");
            sb.Append($"{nameof(OrgId)} : {OrgId}\n");
            sb.Append($"{nameof(ContextOrgId)} : {ContextOrgId}");
            return sb.ToString();
        }
    }
}
