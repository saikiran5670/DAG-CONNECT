using System;

namespace net.atos.daf.ct2.group
{
    public class GroupFilter
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public FunctionEnum FunctionEnum { get; set; }
        public bool GroupRef { get; set; }
        public bool GroupRefCount { get; set; }

        public ObjectType ObjectType { get; set; }

        public GroupType GroupType { get; set; }
    }
}
