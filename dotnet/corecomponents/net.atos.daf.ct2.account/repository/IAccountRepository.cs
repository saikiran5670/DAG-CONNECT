using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account.repository
{
    public interface IAccountRepository
    {
        
         Task<int> AddUser(AccountDetails account);
        //  IEnumerable<AuditLogEntity> GetAuditLogs(int Userorgid);
         Task<int> DeleteUser(int accountid,int loggeduser,bool IsActive);
         Task<IEnumerable<Account>> GetUserDetails(int accountid);
         Task<int> UpdateUser(string firstname,string lastname,int updatedby,int accountid);
         Task<IEnumerable<Account>> GetUsers(int AccounttypeID,bool IsActive);
         Task<int> AddUserRoles(AccountRoleMapping accountRoleMapping);
         Task<bool> CheckEmailExist(string emailid);
       //  Task<IEnumerable<AccountRoleMapping>> GeUserRoles(int Accountorgid,bool IsActive);
         Task<int> DeleteUserRole(int accountorgrolemappingid,int UpdatedBy,bool IsActive);
    }
}
  