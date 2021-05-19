using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.role.entity;

namespace net.atos.daf.ct2.role.repository
{
    public interface IRoleRepository
    {

        Task<IEnumerable<RoleMaster>> GetRoles(RoleFilter roleFilter);
        Task<int> CreateRole(RoleMaster roleMaster);
        Task<int> UpdateRole(RoleMaster roleMaster);
        Task<int> DeleteRole(int roleid, int Accountid);
        // Task<int> CheckRoleNameExist(string roleName);
        Task<int>  Updaterolefeatureset(int RoleId,int FeatureSetId);
        int  CheckRoleNameExist(string roleName,int Organization_Id,int roleid);
        Task<int> IsRoleAssigned(int roleid);
    }
}
