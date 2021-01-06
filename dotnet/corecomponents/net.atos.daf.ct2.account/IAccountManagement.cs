using System;
using net.atos.daf.ct2.account.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.account
{
    public interface IAccountManagement
    {
        Task<int> AddUser(AccountDetails user);
        //  IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid);
         Task<int> DeleteUser(int userid,int loggeduser,bool IsActive);
         Task<IEnumerable<Account>> GetUserDetails(int userid);
         Task<int> UpdateUser(string firstname,string lastname,int updatedby,int userid);
         Task<IEnumerable<Account>> GetUsers(int UsertypeID,bool IsActive);
         Task<int> AddUserRoles(AccountRoleMapping userRoleMapping);
         Task<bool> CheckEmailExist(string emailid);
       //  Task<IEnumerable<AccountRoleMapping>> GetUserRoles(int Userorgid,bool IsActive);
         Task<int> DeleteUserRole(int userorgrolemappingid,int UpdatedBy,bool IsActive);
    }
}
