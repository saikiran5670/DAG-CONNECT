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
        Task<List<Account>> Get(AccountFilter filter);  
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);    
        Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity);
        Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity);
        Task<bool> AddRole(List<AccountRole> accountRoles);
        Task<bool> RemoveRole(AccountRole accountRoles);

    }
}
  