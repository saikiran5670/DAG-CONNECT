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
        Task<Guid?> ResetPasswordInitiate(string emailId);
        Task<bool> ResetPassword(Account account);
        Task<bool> ResetPasswordInvalidate(Guid ResetToken);
        Task<IEnumerable<MenuFeatureDto>> GetMenuFeatures(int accountId, int roleId, int organizationId);
    }
}
