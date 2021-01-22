using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account
{
    public interface IAccountManager
    {
        Task<Account> Create(Account account);
        Task<Account> Update(Account account);
        Task<bool> Delete(Account account);
        Task<bool> ChangePassword(Account account);
        Task<IEnumerable<Account>> Get(AccountFilter filter);  
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);    
        Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity);
        Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity);
        Task<bool> DeleteAccessRelationship(int accountGroupId,int vehicleGroupId);
        Task<bool> AddRole(AccountRole accountRoles);
        Task<bool> RemoveRole(AccountRole accountRoles);
        Task<List<KeyValue>> GetRoles(AccountRole accountRole);
        Task<List<int>> GetRoleAccounts(int roleId);
        Task<List<KeyValue>> GetAccountOrg(int accountId);
        Task<List<KeyValue>> GetAccountRole(int accountId);
    }
}
