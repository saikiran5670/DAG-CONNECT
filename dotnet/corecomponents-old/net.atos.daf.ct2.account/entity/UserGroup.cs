using System;

namespace DAF.Entity
{
    public class UserGroup
    {
        public int UsergroupId { get; set; }
        public int organizationId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy   { get; set; }
        public DateTime CreatedDate { get; set; }        
        public int UpdatedBy   { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Isuserdefinedgroup { get; set; }
    }
}
