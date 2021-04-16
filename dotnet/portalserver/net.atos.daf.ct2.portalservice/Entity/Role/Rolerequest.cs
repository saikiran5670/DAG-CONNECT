using System;

namespace net.atos.daf.ct2.portalservice.Entity.Role
{
    public class Rolerequest
    {
        public int OrganizationId   { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string  Description { get; set; }
        public int[] FeatureIds { get; set; }
        public int Createdby { get; set; }
        public int Level { get; set; }
        public long CreatedAt { get; set; }
        public string Code { get; set; }
    }

    public class Roleupdaterequest
    {
        public int OrganizationId   { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string  Description { get; set; }
        public int[] FeatureIds { get; set; }
        public int Createdby { get; set; }
        public int Updatedby { get; set; }
    }

    public class Rolersponce
    {
        public int OrganizationId   { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string  Description { get; set; }
        public int[] FeatureIds { get; set; }
        public int RoleCount { get; set; }
        public int Createdby { get; set; }
    }
}
