using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account
{
    public interface IAccountRepository
    {
        Task<Account> Create(Account account);
        Task<Account> Update(Account account);
        Task<bool> Delete(int accountId, int organizationId);
        Task<Account> Duplicate(AccountFilter filter);
        Task<List<Account>> Get(AccountFilter filter);
        Task<Account> AddAccountToOrg(Account account);
        Task<AccountBlob> CreateBlob(AccountBlob accountBlob);
        Task<AccountBlob> GetBlob(int blobId);
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);    
        Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity);
        Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity);
        Task<bool> DeleteAccessRelationship(int accountGroupId,int vehicleGroupId);
        Task<bool> AddRole(AccountRole accountRoles);
        Task<bool> RemoveRole(AccountRole accountRoles);
        Task<List<KeyValue>> GetRoles(AccountRole accountRole);
        Task<List<int>> GetRoleAccounts(int roleId);
        Task<List<KeyValue>> GetAccountOrg(int accountId);
        Task<List<AccountOrgRole>> GetAccountRole(int accountId);

    }
}
  