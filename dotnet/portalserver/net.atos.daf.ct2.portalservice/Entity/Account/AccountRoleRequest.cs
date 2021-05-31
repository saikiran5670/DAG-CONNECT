using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountRoleRequest
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public List<int> Roles { get; set; }
    }
    public class AccountRoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
