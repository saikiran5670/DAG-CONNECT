using System;

namespace net.atos.daf.ct2.accountservicerest
{
    public class AccountGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }

        public int RefId { get; set; }
        public string Description { get; set; }
        public int AccountCount { get; set; }
    }

    public class GroupRef
    {
        public int AccountGroupId { get; set; }
        public int AccountId { get; set; }
        
    }
}
