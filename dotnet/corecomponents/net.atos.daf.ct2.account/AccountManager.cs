using net.atos.daf.ct2.audit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Identity = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account
{
    public class AccountManager : IAccountManager
    {
        IAccountRepository repository;
        Identity.IAccountManager identity;
        IAuditTraillib auditlog;
        public AccountManager(IAccountRepository _repository, IAuditTraillib _auditlog, Identity.IAccountManager _identity)
        {
            repository = _repository;
            auditlog = _auditlog;
            identity = _identity;
        }
        public async Task<Account> Create(Account account)  
        {
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            var identityresult = await identity.CreateUser(identityEntity);

            // user already exits in IDP.
            if(identityresult.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
               account.isDuplicate=true;
            }
            if(identityresult.StatusCode == System.Net.HttpStatusCode.Created)
            {
               account = await repository.Create(account);
            }
            else // there is issues delete user from IDP. 
            {
              identityresult  = await identity.DeleteUser(identityEntity);
             if(identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    // check to handle message
                }
            }            
            return account;
        }
        public async Task<Account> Update(Account account)  
        {
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            var identityresult = await identity.UpdateUser(identityEntity);

            if(identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
               account = await repository.Update(account);
            }            
            return account;
        }
        public async Task<bool> Delete(Account account)
        {
            bool result=false;
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            var identityresult = await identity.DeleteUser(identityEntity);
            if(identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
               result = await repository.Delete(account.Id,account.Organization_Id);
            }       
            return result;
        }
        public async Task<bool> ChangePassword(Account account)  
        {
            bool result=false;
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            identityEntity.Password = account.Password;
            var identityresult = await identity.ChangeUserPassword(identityEntity);
            if(identityresult.StatusCode == System.Net.HttpStatusCode.OK)
            {
               result = true;
            }
            return result;
        }
        public async Task<IEnumerable<Account>> Get(AccountFilter filter)
        {
            return await repository.Get(filter);
        }
        public async Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity)
        {
            return await repository.CreateAccessRelationship(entity);
        }
        public async Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity)
        {
            return await repository.UpdateAccessRelationship(entity);
        }
        public async Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter)
        {
            return await repository.GetAccessRelationship(filter);
        }
        public async  Task<bool> AddRole(List<AccountRole> accountRoles)
        {
            return await repository.AddRole(accountRoles);
        }
        public async  Task<bool> RemoveRole(AccountRole accountRoles)
        {
            return await repository.RemoveRole(accountRoles);
        }
        public async  Task<List<string>> GetRoles(AccountRole accountRoles)
        {
            return await repository.GetRoles(accountRoles);
        }   
        public async  Task<List<int>> GetRoleAccounts(int roleId)
        {
            return await repository.GetRoleAccounts(roleId);
        }   
        
    }
}
