using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.user.entity;

namespace net.atos.daf.ct2.user.repository
{
    public interface IUserRepository
    {
        
         Task<int> AddUser(UserDetails user);
        //  IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid);
         Task<int> DeleteUser(int userid,int loggeduser,bool IsActive);
         Task<IEnumerable<User>> GetUserDetails(int userid);
         Task<int> UpdateUser(string firstname,string lastname,int updatedby,int userid);
         Task<IEnumerable<User>> GetUsers(int UsertypeID,bool IsActive);
         Task<int> AddUserRoles(UserRoleMapping userRoleMapping);
         Task<bool> CheckEmailExist(string emailid);
         Task<IEnumerable<UserRoleMapping>> GetUserRoles(int Userorgid,bool IsActive);
         Task<int> DeleteUserRole(int userorgrolemappingid,int UpdatedBy,bool IsActive);
    }
}
  