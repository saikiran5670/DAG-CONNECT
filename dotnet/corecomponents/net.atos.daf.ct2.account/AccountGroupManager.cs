// using System;
// using net.atos.daf.ct2.account.repository;
// using net.atos.daf.ct2.account.entity;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace net.atos.daf.ct2.account
// {
//     public class AccountGroupManager:IAccountGroupManager
//     {
        
//         IAccountGrouprepository usergroupRepository;
//         public AccountGroupManager(IAccountGrouprepository _usergroupRepository)
//         {
//             usergroupRepository = _usergroupRepository;
//         }

//         public int AddUserGroup(AccountGroup usergroup)
//         {
//             try
//             {
//                 return usergroupRepository.AddUserGroup(usergroup);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }

//         public int UpdateUserGroup(AccountGroup usergroup)
//         {
//             try
//             {
//                 return usergroupRepository.UpdateUserGroup(usergroup);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }

//       public int DeleteUserGroup(int userGroupId, int Updatedby,bool IsActive)
//         {
//             try
//             {
//                 return usergroupRepository.DeleteUserGroup(userGroupId,Updatedby,IsActive);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }

//         public IEnumerable<AccountGroup> GetUserGroups(int organizationId,bool IsActive)
//         {
//             try
//             {
//                 return usergroupRepository.GetUserGroups(organizationId,IsActive);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }

//         public  IEnumerable<AccountGroup> GetUserGroupDetails(int UserGroupID,int organizationId)
//         {
//             try
//             {
//                 return usergroupRepository.GetUserGroupDetails(UserGroupID,organizationId);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }

//         public async Task<int> AddUserGroupRoles(AccountGroupRole usergrpRoleMapping)
//         {
//             try
//             {
//                 return await usergroupRepository.AddUserGroupRoles(usergrpRoleMapping);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }
//         public async Task<IEnumerable<AccountGroupRole>> GetUserGroupRoles(int UsergroupId,bool IsActive)
//         {
//             try
//             {
//                 return await usergroupRepository.GetUserGroupRoles(UsergroupId,IsActive);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }

//         public async Task<int> DeleteUserGroupRole(int UserGroupRolemappingid,int UpdatedBy,bool IsActive)
//         {
//             try
//             {
//                 return await usergroupRepository.DeleteUserGroupRole(UserGroupRolemappingid,UpdatedBy,IsActive);
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }

//         }

//     }
// }
