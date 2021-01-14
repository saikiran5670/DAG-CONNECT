using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.role.entity;

namespace net.atos.daf.ct2.role.repository
{
    public interface IRoleRepository
    {

        Task<IEnumerable<RoleMaster>> GetRoles(int roleid);
        Task<int> CreateRole(RoleMaster roleMaster);
        Task<int> UpdateRole(RoleMaster roleMaster);
        Task<int> DeleteRole(int roleid, int Accountid);
        Task<int> CheckRoleNameExist(string roleName);
    }
}
