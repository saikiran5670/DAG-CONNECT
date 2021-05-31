using System.Collections.Generic;
namespace net.atos.daf.ct2.group
{

    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObjectType ObjectType { get; set; }
        public GroupType GroupType { get; set; }
        public string Argument { get; set; }
        public FunctionEnum FunctionEnum { get; set; }
        public int OrganizationId { get; set; }
        public int? RefId { get; set; }
        public string Description { get; set; }
        public List<GroupRef> GroupRef { get; set; }
        public int GroupRefCount { get; set; }
        public bool Exists { get; set; }
        public long? CreatedAt { get; set; }

        public Group() { }

        public Group(GroupType groupType, ObjectType objectType, string argument, FunctionEnum functionEnum, int refId, string groupName, string description, long createat, int orgnizationId)
        {
            // initilize group
            GroupType = groupType;
            ObjectType = objectType;
            Argument = argument;
            FunctionEnum = functionEnum;
            RefId = refId;
            Description = description;
            CreatedAt = createat;
            Name = groupName;
            OrganizationId = orgnizationId;
        }
    }
}
