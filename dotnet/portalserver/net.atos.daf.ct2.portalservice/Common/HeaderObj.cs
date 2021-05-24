namespace net.atos.daf.ct2.portalservice.Common
{
    public class HeaderObj
    {
        public int roleId { get; set; }
        public int accountId { get; set; }
        public int orgId { get; set; }
        public int contextOrgId
        {
            get
            {
                return orgId;
            }
            set
            {
                contextOrgId = value;
            }
        }            
    }
}
