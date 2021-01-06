using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.user.entity;

namespace net.atos.daf.ct2.user.repository
{
    public interface IUserGrouprepository
    {
       int AddUserGroup(UserGroup usergroup);
       int UpdateUserGroup(UserGroup usergroup);
       int DeleteUserGroup(int usergroupId,int UpdatedBy,bool IsActive);
       IEnumerable<UserGroup> GetUserGroups(int organizationId,bool IsActive);
       IEnumerable<UserGroup> GetUserGroupDetails(int UserGroupID,int organizationId);
       Task<int> AddUserGroupRoles(UserGroupRole userRoleMapping);
       Task<IEnumerable<UserGroupRole>> GetUserGroupRoles(int UsergroupId,bool IsActive);
       Task<int> DeleteUserGroupRole(int UserGroupRolemappingid,int UpdatedBy,bool IsActive);
    }
}
