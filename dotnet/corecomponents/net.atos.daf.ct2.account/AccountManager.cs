using net.atos.daf.ct2.audit;
using System.Linq;
using System.Collections.Generic;
using System;
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
            identityEntity.EmailId = account.EmailId;            
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            identityEntity.Password = account.Password;

            //TODO: If created in IDP, but have exception while create in DB.
            var identityresult = await identity.CreateUser(identityEntity);

            if (identityresult.StatusCode == System.Net.HttpStatusCode.Created)
            {
                // if this fails
                account = await repository.Create(account);
            }
            else // there is issues and need delete user from IDP. 
            {
                // user already exits in IDP.
                if (identityresult.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    account.isDuplicate = true;
                    // get account by email , if not exists in DB-- create it
                    AccountFilter filter = new AccountFilter();
                    filter.Email = account.EmailId;
                    filter.OrganizationId =0 ;        
                    filter.AccountType = AccountType.None;
                    filter.AccountIds = string.Empty;
                    filter.Name = string.Empty;
                    var result = await repository.Get(filter);
                    var accountGet = result.FirstOrDefault();
                    if (accountGet == null)
                    {
                        account = await repository.Create(account);
                        await identity.UpdateUser(identityEntity);
                        account.isDuplicate = false;
                    }
                    // else 
                    // {
                    //     if ( Convert.ToInt32(accountGet.Id) <=0 )
                    //     {
                    //         account = await repository.Create(account);
                    //     }
                    // }
                }
                // inter server error in IDP.
                else if (identityresult.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    account.isError = true;
                }
                else if (identityresult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    account.isError = true;
                }
                //identityresult = await identity.DeleteUser(identityEntity);
                if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
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
            //identityEntity.EmailId = account.EmailId;    
            identityEntity.FirstName = account.FirstName;
            identityEntity.LastName = account.LastName;
            var identityresult = await identity.UpdateUser(identityEntity);

            if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                account = await repository.Update(account);
            }
            return account;
        }
        public async Task<bool> Delete(Account account)
        {
            bool result = false;
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            // identityEntity.EmailId = account.EmailId;
            // identityEntity.FirstName = account.FirstName;
            // identityEntity.LastName = account.LastName;
            var identityresult = await identity.DeleteUser(identityEntity);
            if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                result = await repository.Delete(account.Id, account.Organization_Id);
            }
            else if (identityresult.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                    //TODO:  need to handle this -- is there in DB but not is IDP.
            }
            return result;
        }
        public async Task<bool> ChangePassword(Account account)
        {
            bool result = false;
            // create user in identity
            IdentityEntity.Identity identityEntity = new IdentityEntity.Identity();
            identityEntity.UserName = account.EmailId;
            //identityEntity.EmailId = account.EmailId;
            // identityEntity.FirstName = account.FirstName;
            // identityEntity.LastName = account.LastName;
            identityEntity.Password = account.Password;
            var identityresult = await identity.ChangeUserPassword(identityEntity);
            if (identityresult.StatusCode == System.Net.HttpStatusCode.NoContent)
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
        public async Task<bool> DeleteAccessRelationship(int accountGroupId,int vehicleGroupId)
        {
            return await repository.DeleteAccessRelationship(accountGroupId,vehicleGroupId);
        }
        public async Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter)
        {
            return await repository.GetAccessRelationship(filter);
        }
        public async Task<bool> AddRole(AccountRole accountRoles)
        {
            return await repository.AddRole(accountRoles);
        }
        public async Task<bool> RemoveRole(AccountRole accountRoles)
        {
            return await repository.RemoveRole(accountRoles);
        }
        public async Task<List<KeyValue>> GetRoles(AccountRole accountRoles)
        {
            return await repository.GetRoles(accountRoles);
        }
        public async Task<List<int>> GetRoleAccounts(int roleId)
        {
            return await repository.GetRoleAccounts(roleId);
        }

        public async Task<List<KeyValue>> GetAccountOrg(int accountId)
        {
            return await repository.GetAccountOrg(accountId);
        }
        public async Task<List<KeyValue>> GetAccountRole(int accountId)
        {
            return await repository.GetAccountRole(accountId);
        }

    }
}
