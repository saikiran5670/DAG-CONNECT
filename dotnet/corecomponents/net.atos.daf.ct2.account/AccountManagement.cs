using System;
using net.atos.daf.ct2.account.repository;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.audit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.account
{
    public class AccountManagement:IAccountManagement
    {
        IAccountRepository userRepository;        
        IAuditLog auditlog;
        public AccountManagement(IAccountRepository _userRepository,IAuditLog _auditlog)
        {
            userRepository = _userRepository;
            auditlog=_auditlog;
        }
         public async Task<int> AddUser(AccountDetails userDetails)
        {
            try
            {
                int UserId= await userRepository.AddUser(userDetails);
                auditlog.AddLogs(1,1,1,"User Insert",UserId > 0,"User Management", "User Added With User Id " + UserId.ToString());
                return UserId;              

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteUser(int userid,int loggeduser,bool IsActive)
        {
            try
            {
                int UserId= await userRepository.DeleteUser(userid,loggeduser,IsActive);
                auditlog.AddLogs(1,1,userid,"User Delete",UserId > 0,"User Management", "User Deleted With User Id " + UserId.ToString());
                return UserId;                   
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public Task<bool> CheckEmailExist(string emailid)
        {
            return userRepository.CheckEmailExist(emailid);
        }

         public async Task<IEnumerable<Account>> GetUserDetails(int userid)
        {
            try
            {
                return await userRepository.GetUserDetails(userid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateUser(string firstname,string lastname,int updatedby,int userid)
        {
            try
            {
                int UserId= await userRepository.UpdateUser(firstname,lastname,updatedby,userid);
                auditlog.AddLogs(1,1,userid,"User Update",UserId > 0,"User Management", "User Updated With User Id " + userid.ToString());
                return UserId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Account>> GetUsers(int UsertypeID,bool IsActive)
        {
            try
            {
                return await userRepository.GetUsers(UsertypeID,IsActive);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AddUserRoles(AccountRoleMapping userRoleMapping)
        {
             try
            {
                return await userRepository.AddUserRoles(userRoleMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // public async Task<IEnumerable<AccountRoleMapping>> GetUserRoles(int Userorgid,bool IsActive)
        // {
        //     try
        //     {
        //         return await userRepository.GetUserRoles(Userorgid,IsActive);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }

        public async Task<int> DeleteUserRole(int userorgrolemappingid,int UpdatedBy,bool IsActive)
        {
            try 
            {
                return await userRepository.DeleteUserRole(userorgrolemappingid,UpdatedBy,IsActive);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        
    }
}
