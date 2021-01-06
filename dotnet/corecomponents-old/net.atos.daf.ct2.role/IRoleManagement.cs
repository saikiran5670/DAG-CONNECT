using System;
using net.atos.daf.ct2.role.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rolerepository
{
    public interface IRoleManagement
    {
         Task<IEnumerable<RoleMaster>> GetRoles(int roleid);
        Task<int> AddRole(RoleMaster roleMaster);
        Task<int> UpdateRole(RoleMaster roleMaster);
        Task<int> DeleteRole(int roleid,int userid);
        Task<int> CheckRoleNameExist(string roleName);
    }
}
