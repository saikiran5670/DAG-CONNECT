using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }
        public int RefId { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }
        //public string FunctionEnum { get; set; }
        //public int AccountCount { get; set; }
        public List<GroupRef> Accounts { get; set; }
    }
    public class AccountGroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string ObjectType { get; set; }
        public string GroupType { get; set; }
        //public string Argument { get; set; }
        //public string FunctionEnum { get; set; }
        public int OrganizationId { get; set; }
        public int? RefId { get; set; }
        public string Description { get; set; }
        public List<GroupRef> GroupRef { get; set; }
        public int GroupRefCount { get; set; }
    }

    public class GroupRef
    {
        public int AccountGroupId { get; set; }
        public int AccountId { get; set; }

    }
    public class AccountGroupAccount
    {
        public List<GroupRef> Accounts { get; set; }
    }
}
