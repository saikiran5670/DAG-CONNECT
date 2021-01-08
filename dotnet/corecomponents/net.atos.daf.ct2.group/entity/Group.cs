using System;
using System.Collections.Generic;
namespace net.atos.daf.ct2.group
{
   
    public class Group 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObjectType ObjType { get; set; }
        public GroupType GroupType { get; set; }
        public string Argument { get; set; }
        public FunctionEnum FunctionEnum { get; set; }        
        public int OrganizationId { get; set; }
        public int ? RefId { get; set; }
        public string Description { get; set; }
        public List<GroupRef> GroupRef { get; set; }
        public int GroupRefCount { get; set; }

   
    }
}
