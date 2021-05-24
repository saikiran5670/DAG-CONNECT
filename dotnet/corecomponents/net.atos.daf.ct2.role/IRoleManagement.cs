using System;
using net.atos.daf.ct2.role.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.role
{
    public interface IRoleManagement
    {
        Task<IEnumerable<RoleMaster>> GetRoles(RoleFilter rolefilter);
        Task<int> CreateRole(RoleMaster roleMaster);
        Task<int> UpdateRole(RoleMaster roleMaster);
      Task<int> DeleteRole(int roleid, int Accountid);
        int  CheckRoleNameExist(string roleName,int Organization_Id,int roleid);
        Task<IEnumerable<AssignedRoles>> IsRoleAssigned(int roleid);
    }
}
