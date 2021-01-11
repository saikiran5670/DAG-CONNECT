// using System;
// using net.atos.daf.ct2.account.entity;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace net.atos.daf.ct2.account
// {
//     public interface IAccountGroupManager
//     {
//        int AddUserGroup(AccountGroup usergroup);
//        int UpdateUserGroup(AccountGroup usergroup);
//        int DeleteUserGroup(int usergroupId,int UpdatedBy,bool IsActive);
//        IEnumerable<AccountGroup> GetUserGroups(int organizationId,bool IsActive);
//        IEnumerable<AccountGroup> GetUserGroupDetails(int UserGroupID,int organizationId);

//        Task<int> AddUserGroupRoles(AccountGroupRole usergrpRoleMapping);

//        Task<IEnumerable<AccountGroupRole>> GetUserGroupRoles(int UsergroupId,bool IsActive);
//        Task<int> DeleteUserGroupRole(int UserGroupRolemappingid,int UpdatedBy,bool IsActive);
//     }
// }
