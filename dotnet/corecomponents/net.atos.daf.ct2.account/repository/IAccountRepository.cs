using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.identitysession.entity;

namespace net.atos.daf.ct2.account
{
    public interface IAccountRepository
    {
        Task<Account> Create(Account account);
        Task<Account> Update(Account account);
        Task<bool> Delete(int accountId, int organizationId);
        Task<Account> Duplicate(AccountFilter filter);
        Task<List<Account>> Get(AccountFilter filter);
        Task<Account> GetAccountByEmailId(string emailId);
        Task<Account> GetAccountByAccountId(int accountId);
        Task<int> GetCount(int organization_id);
        Task<Account> AddAccountToOrg(Account account);
        Task<int> UpsertPasswordModifiedDate(int accountId, long modifiedAt);
        Task<int> UpsertPasswordPolicyAccount(PasswordPolicyAccount passwordPolicyForBlockAccount);
        Task<long?> GetPasswordModifiedDate(int accountId);
        Task<PasswordPolicyAccount> GetPasswordPolicyAccount(int accountId);
        Task<AccountBlob> CreateBlob(AccountBlob accountBlob);
        Task<AccountBlob> GetBlob(int blobId);
        Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter);
        Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity);
        Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity);
        Task<bool> DeleteAccessRelationship(int accountGroupId, int vehicleGroupId);
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
        Task<ResetPasswordToken> Create(ResetPasswordToken resetPasswordToken);
        Task<int> Update(int id, ResetTokenStatus status);
        Task<ResetPasswordToken> GetIssuedResetToken(Guid tokenSecret);
        Task<ResetPasswordToken> GetIssuedResetTokenByAccountId(int accountId);
        Task<IEnumerable<MenuFeatureDto>> GetMenuFeaturesList(MenuFeatureRquest request);
        Task<bool> CheckForFeatureAccessByEmailId(string emailId, string featureName);
        Task<string> GetCodeByPreferenceId(int preferenceId);
        Task<string> GetLanguageCodePreference(string emailId, int? orgId);
        Task<IEnumerable<Account>> GetAccountOfPasswordExpiry(int noOfDays);
        Task<int> UpdateIsReminderSent(int accountId, bool isReminderSend = true);
        Task<List<SSOTokenResponse>> GetAccountSSODetails(AccountToken AccountID);
    }
}
