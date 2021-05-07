using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.email.Enum;

namespace net.atos.daf.ct2.account
{
    public interface IAccountManager
    {
        Task<Account> Create(Account account);
        Task<Account> Update(Account account);
        Task<bool> Delete(Account account);
        Task<Response> ChangePassword(Account account);
        Task<IEnumerable<Account>> Get(AccountFilter filter);
        Task<int> GetCount(int organization_id);
        Task<Account> AddAccountToOrg(Account account);
        Task<Account> GetAccountByEmailId(string emailId);
        Task<AccountBlob> CreateBlob(AccountBlob accountBlob);
        Task<AccountBlob> GetBlob(int blobId);
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);    
        Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity);
        Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity);
        Task<bool> DeleteAccessRelationship(int accountGroupId,int vehicleGroupId);        
        Task<List<AccountVehicleAccessRelationship>> GetAccountVehicleAccessRelationship(AccountVehicleAccessRelationshipFilter filter, bool is_vehicleGroup);
        Task<List<AccountVehicleEntity>> GetVehicle(AccountVehicleAccessRelationshipFilter filter, bool is_vehicle);
        Task<List<AccountVehicleEntity>> GetAccount(AccountVehicleAccessRelationshipFilter filter, bool is_account);
        Task<bool> DeleteVehicleAccessRelationship(int organizationId, int groupId, bool isVehicle);
        Task<bool> AddRole(AccountRole accountRoles);
        Task<bool> RemoveRole(AccountRole accountRoles);
        Task<List<KeyValue>> GetRoles(AccountRole accountRole);
        Task<List<int>> GetRoleAccounts(int roleId);
        Task<List<KeyValue>> GetAccountOrg(int accountId);
        Task<List<AccountOrgRole>> GetAccountRole(int accountId);
        Task<Response> ResetPasswordInitiate(string emailId, int orgId, EmailEventType eventType = EmailEventType.ResetPassword);
        Task<Response> ResetPassword(Account account);
        Task<Response> ResetPasswordInvalidate(Guid ResetToken);
        Task<IEnumerable<MenuFeatureDto>> GetMenuFeatures(int accountId, int roleId, int organizationId, string languageCode);
        Task<bool> CheckForFeatureAccessByEmailId(string emailId, string featureName);
        Task<PasswordPolicyAccount> GetPasswordPolicyAccount(int id);
        Task<int> UpsertPasswordPolicyAccount(PasswordPolicyAccount passwordPolicyBlockAccount);
        Task<Response> GetResetPasswordTokenStatus(Guid processToken);
        Task<IEnumerable<EmailList>> SendEmailForPasswordExpiry(int noOfDays);
    }
}
