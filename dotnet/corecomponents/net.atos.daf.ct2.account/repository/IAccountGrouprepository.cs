using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account.repository
{
    public interface IAccountGrouprepository
    {
       int AddUserGroup(AccountGroup accountgroup);
       int UpdateUserGroup(AccountGroup accountgroup);
       int DeleteUserGroup(int accountgroupId,int UpdatedBy,bool IsActive);
       IEnumerable<AccountGroup> GetUserGroups(int organizationId,bool IsActive);
       IEnumerable<AccountGroup> GetUserGroupDetails(int AccountGroupID,int organizationId);
       Task<int> AddUserGroupRoles(AccountGroupRole accountRoleMapping);
       Task<IEnumerable<AccountGroupRole>> GetUserGroupRoles(int AccountgroupId,bool IsActive);
       Task<int> DeleteUserGroupRole(int AccountGroupRolemappingid,int UpdatedBy,bool IsActive);
    }
}
