using System;

namespace DAF.Entity
{
    public class UserGroupRole
    {
        public int UserGroupRolemappingid { get; set; }
        public int UserGroupID { get; set; }
        public int Userorgid { get; set; }
        public int rolemasterid { get; set; }
        public bool IsActive { get; set; } 
        public int CreatedBy   { get; set; }
        public DateTime CreatedDate { get; set; }        
        public int UpdatedBy   { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime RoleStartdate { get; set; }
        public DateTime RoleEnddate { get; set; }
    }
}
